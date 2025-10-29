import { $viewer, applyAuthBarrier } from '~/shared/auth.ts';
import { createEffect, createEvent, createStore, sample } from 'effector';
import { z } from 'zod';
import { createForm } from '~/shared/lib/form';
import { createApiEffectWithContract } from '~/shared/api/client';
import { components } from '~/shared/api/schema';
import { $product, profileProductsRoute, setIsVisibleModal } from '~/pages/profile-products/model';
import { profileProductsNewRoute, setOpenedDirtyPage } from '~/pages/profile-products-new/model';
import { routes } from '~/shared/routing';
import { ProductIn } from './validation';
import { debounce, debug } from 'patronum';
import { createMutation, createQuery } from '@farfetched/core';
import { showToast } from '~/shared/notifications';

export const setActiveProductId = createEvent<string | null>();
export const addDraftEvent = createEvent<DraftParams>();
export const updateDraftEvent = createEvent();
export const fetchSuggestionsRaw = createEvent<string>();
const setModalHandled = createEvent<boolean>();

const isModalHandled = createStore(false).on(setModalHandled, (_, payload) => payload);
export const $suggestions = createStore<components['schemas']['GetAccountSummaryOut'][]>([]);
export const $activeProductId = createStore<string | null>(null).on(setActiveProductId, (_, payload) => payload);
export const $activeView = createStore<React.ComponentType | null>(null);
export const $itemsProducts = createStore<string[]>(['Общая информация']);
export const $activeAccount = $viewer.map(
  (q) => q?.accounts?.find((account) => account.id === q.activeAccountId) || null,
);
export const productsOptionsQuery = createQuery({
  ...createApiEffectWithContract('get', '/api/v1/products/options'),
  mapData: (data: { result: components['schemas']['ProductOptionsOut'] }) => data.result,
});
applyAuthBarrier(productsOptionsQuery);
export const searchAccountsByNameMutation = createMutation({
  ...createApiEffectWithContract('get', '/api/v1/accounts/{name}', {
    mapParams: ({ name }: { name: string }) => {
      return { params: { path: { name } } };
    },
  }),
});
applyAuthBarrier(searchAccountsByNameMutation);
export const productAccountQuery = createQuery({
  ...createApiEffectWithContract('get', '/api/v1/account/product/{id}', {
    mapParams: ({ id }: { id: string }) => {
      return { params: { path: { id } } };
    },
  }),
  mapData: ({ result }: { result: any }) => result,
});
applyAuthBarrier(productAccountQuery);
export const addProductMutation = createMutation({
  ...createApiEffectWithContract('post', '/api/v1/product', {
    mapParams: (args: components['schemas']['AddProductDraftIn']) => {
      return { body: args };
    },
  }),
});
applyAuthBarrier(addProductMutation);
export const updateProductMutation = createMutation({
  ...createApiEffectWithContract('patch', '/api/v1/product/{id}', {
    mapParams: (args: components['schemas']['UpdateProductIn'] & { id: string }) => {
      const { id, ...body } = args;
      return {
        params: { path: { id: id } },
        body,
      };
    },
  }),
});
applyAuthBarrier(updateProductMutation);
export const productImagesUploadMutation = createMutation({
  handler: async ({ id, files, options }: { id: string; files: File[]; options: { clear: () => void } }) => {
    const formData = new FormData();
    for (const file of files) {
      formData.append('files', file, file.name);
    }

    await fetch(`/api/v1/product/images/${id}`, {
      method: 'POST',
      body: formData,
    });

    options.clear();
  },
});
applyAuthBarrier(productImagesUploadMutation);

sample({
  clock: [productAccountQuery.finished.success, addProductMutation.finished.success],
  target: createEffect(() => {
    if (profileProductsRoute.opened) {
      setOpenedDirtyPage(false);
      routes.profile['products-new'].open();
    }
  }),
});

export const productForm = createForm<ProductIn>({
  defaultValues: {
    name: '',
    description: '',
    website: '',
    typeId: '',
    images: [],
    rightholders: [],
    productData: {
      VideoUrl: '',
      // // step-1 block-1
      // applicationType: 'General',
      // mainProduct: [],
      // // step-1 block-2
      // icon: '',
      // // step-1 block-3
      // lifecycleStage: [],
      // businessProcess: '',
      // class: '',
      // impactObject: [],
      // projectMark: [],
      // // step-1 block-4
      // worksWithCIM: false,
      // // step-1 block-5
      // inNOTIM: true,
      // inRegistry: false,
      // registryCode: '',
      // registryName: '',
      // registryLink: '',
      // // step-2 block-1
      // media: [],
      // // step-2 block-2
      // language: [],
      // os: [],
      // releaseDate: new Date(),
      // updateDate: new Date(),
      // versionInfo: '',
      // customerSupport: '',
      // // step-3
      // subscriptions: [], // subscriptions are commented out in schema
    },
  },
  validators: {
    onSubmit: ProductIn,
  },
});
// biome-ignore lint/suspicious/noRedeclare: <explanation>
export type ProductIn = z.infer<typeof ProductIn>;
interface DraftParams {
  name?: string;
  type?: string;
}
{
  // Save and update draft product
  sample({
    clock: addDraftEvent,
    source: productForm.$values,
    fn: (productValues, eventParams) => {
      if (!productValues) return;
      const { name, typeId } = productValues;
      const finalName = name && name.trim() !== '' ? name : eventParams?.name;
      const finalType = typeId && typeId.trim() !== '' ? typeId : eventParams?.type;
      return {
        name: finalName,
        typeId: finalType,
      } as any;
    },
    target: addProductMutation.start,
  });
  sample({
    clock: updateDraftEvent,
    source: {
      formValues: productForm.$values,
      id: $activeProductId,
    },
    fn: ({ formValues, id }) => {
      if (!formValues) return;
      const { productData, rightholders, images, ...restFields } = formValues;
      const rightholdersItems = rightholders as components['schemas']['GetAccountSummaryOut'][];
      const rightholdersIds = Array.isArray(rightholdersItems)
        ? rightholdersItems.map((item) => item.id).filter(Boolean)
        : [];

      return {
        updateProductPayload: {
          id,
          ...restFields,
          rightholders: rightholdersIds,
          productData: typeof productData === 'object' && !Array.isArray(productData) ? productData : {},
        },
        updateImagesPayload: {
          id: id!,
          files: images || [],
          options: {
            clear: () => {},
          },
        },
      };
    },
    target: [
      updateProductMutation.start.prepend((payload?: { updateProductPayload?: any }) => payload?.updateProductPayload),
      productImagesUploadMutation.start.prepend(
        (payload?: { updateImagesPayload?: any }) => payload?.updateImagesPayload,
      ),
    ],
  });
}

// Resets product form
function normalizeProductData(productData: any) {
  if (!productData || typeof productData !== 'object') return {};

  const entries = Object.values(productData) as Array<{ attributeName?: string; value?: any }>;
  const normalized = entries.reduce(
    (acc, item) => {
      if (item?.attributeName && item?.value !== undefined) {
        acc[item.attributeName] = item.value;
      }
      return acc;
    },
    {} as Record<string, any>,
  );
  return normalized;
}

sample({
  clock: [productAccountQuery.finished.success, addProductMutation.finished.success],
  fn: ({ result }: { result: components['schemas']['GetProductOut'] }) => {
    if (!result) return {};
    return {
      ...result,
      productData: normalizeProductData(result.productData),
      images: Array.isArray(result.images)
        ? result.images.map((imgObj: components['schemas']['AvatarOut']) => imgObj.url)
        : [],
    } as any;
  },
  target: productForm.reset,
});

// Add new product
sample({
  clock: [addProductMutation.finished.success, productAccountQuery.finished.success],
  fn: ({ result }) =>
    result && typeof result === 'object' && 'id' in result ? (result as { id: string | null }).id : null,
  target: setActiveProductId,
});

export const fetchSuggestions = debounce({
  source: fetchSuggestionsRaw,
  timeout: 500,
});
sample({
  source: fetchSuggestions,
  fn: (query: any) => ({ name: query }),
  target: searchAccountsByNameMutation.start,
});
sample({
  source: searchAccountsByNameMutation.finished.success,
  fn: (src) => src.result as components['schemas']['GetAccountSummaryOut'][],
  target: $suggestions,
});
sample({
  clock: updateProductMutation.finished.success,
  fn: () => {
    return {
      severity: 'success',
      life: 3000,
      summary: 'Внесенные изменения успешно сохранены',
    } as const;
  },
  target: showToast,
});

sample({
  clock: profileProductsNewRoute.opened,
  filter: isModalHandled.map((handled) => !handled),
  fn: () => {},
  target: productsOptionsQuery.start,
});

sample({
  clock: setIsVisibleModal,
  filter: (isVisible) => isVisible,
  fn: () => {},
  target: [productsOptionsQuery.start, setModalHandled.prepend(() => true)],
});
