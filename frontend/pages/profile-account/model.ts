import { createMutation, createQuery, update } from '@farfetched/core';
import { createApiEffectWithContract } from '~/shared/api/client';
import { $viewer, applyAuthBarrier, chainAuthorized, checkViewerQuery, viewerQuery } from '~/shared/auth.ts';
import { controls, routes } from '~/shared/routing.ts';
import { $firstName, $lastName } from '../profile/model';
import { combine, createEffect, createEvent, createStore, merge, sample } from 'effector';
import { components } from '~/shared/api/schema';
import { z } from 'zod';
import { createForm, submit, updateErrors } from '~/shared/lib/form';
import { mapValidationErrorToForm } from '~/shared/lib/form-helpers';
import { FileUploadHandlerEvent } from 'primereact/fileupload';
import { debounce, debug } from 'patronum';
import { querySync } from 'atomic-router';
import { NewPrivateAccount } from './ui/private-account';
import { showToast } from '~/shared/notifications';

export const profileAccountRoute = chainAuthorized(routes.profile.account);

type AddAccountIn = components['schemas']['AddAccountIn'];
type UpdateBrandDto = components['schemas']['UpdateBrandIn'];

export const accountQuery = createQuery({
  ...createApiEffectWithContract('get', '/api/v1/account'),
  mapData: (data) => data.result,
  initialData: {
    id: '',
    status: 'Pending',
    type: 'Regular',
    inn: '',
    fullName: '',
    address: '',
    kpp: '',
    ogrn: '',
    options: {
      publicLists: [],
      mainActivities: [],
    },
    createdAt: '',
    updatedAt: '',
    isDeveloper: false,
  },
});
applyAuthBarrier(accountQuery);

export const STATUS_MAP: Record<string, { label: string; severity: 'success' | 'warning' }> = {
  Accepted: { label: 'Активный', severity: 'success' },
  Invited: { label: 'Приглашён', severity: 'warning' },
};

export const ROLE_MAP = {
  Owner: 'Владелец',
  Manager: 'Менеджер',
  Developer: 'Разработчик',
  Member: 'Участник',
} as const;

export type AccountRole = keyof typeof ROLE_MAP;
export function mapInvitations(profiles: components['schemas']['InvitationsOut'][] = []): {
  id: string;
  user: string;
  avatar?: string;
  email?: string;
  roleKey: string;
  roleLabel: string;
  status: string;
  res: any;
}[] {
  return profiles.map((res) => {
    const user = res.profile ? `${res.profile.lastName ?? ''} ${res.profile.firstName ?? ''}`.trim() : 'Без имени';

    return {
      id: res.id ?? '',
      res,
      user,
      avatar: res.profile?.avatar.url ?? undefined,
      email: res.email ?? undefined,
      roleKey: res.role ?? '',
      roleLabel: res.role && res.role in ROLE_MAP ? ROLE_MAP[res.role as keyof typeof ROLE_MAP] : (res.role ?? ''),
      status: res.status ?? '',
    };
  });
}

type BaseAccountArgs = AddAccountIn & {
  firstName?: string;
  lastName?: string;
  middleName?: string;
  fullName?: string;
  shortName?: string;
  accountId?: string;
};

export function mapAccountParams(args: BaseAccountArgs, isUpdate = false) {
  const { firstName = '', lastName = '', middleName = '', fullName, shortName, type, accountId, ...rest } = args;

  const fio = [lastName, firstName, middleName].filter(Boolean).join(' ').trim();
  const finalFullName = (fullName ?? fio).trim();
  const finalShortName = (shortName ?? (type?.toLowerCase() === 'individualentrepreneur' ? `ИП ${fio}` : fio)).trim();

  const result: any = {
    body: {
      type,
      ...rest,
      fullName: finalFullName,
      shortName: finalShortName,
    },
  };

  if (isUpdate && accountId) {
    result.params = { path: { accountId } };
  }

  return result;
}

export const addAccountMutation = createMutation({
  ...createApiEffectWithContract('post', '/api/v1/account', {
    mapParams: (args: BaseAccountArgs) => mapAccountParams(args),
  }),
});

export const updateAccountMutation = createMutation({
  ...createApiEffectWithContract('put', '/api/v1/account/{accountId}', {
    mapParams: (args: BaseAccountArgs) => mapAccountParams(args, true),
  }),
});

export const updateBrandMutation = createMutation({
  ...createApiEffectWithContract('patch', '/api/v1/brand', {
    mapParams: (args: UpdateBrandDto) => {
      return { body: args };
    },
  }),
});
applyAuthBarrier(updateBrandMutation);
export const searchInnMutation = createMutation({
  ...createApiEffectWithContract('get', '/api/v1/inn-search', {
    mapParams: (args: { query: string }) => ({
      params: { query: { query: args.query } },
    }),
  }),
});
applyAuthBarrier(searchInnMutation);
export const avatarUploadMutation = createMutation({
  handler: async (file: File) => {
    const formData = new FormData();
    formData.append('imageFile', file, file.name);

    const res = await fetch('/api/v1/brand/avatar', {
      method: 'POST',
      body: formData,
    });

    if (!res.ok) {
      throw new Error('Upload failed');
    }
  },
});
applyAuthBarrier(avatarUploadMutation);

export const changeAccountMutation = createMutation({
  ...createApiEffectWithContract('put', '/api/v1/active-account', {
    mapParams: (args: { accountId?: string | null }) => ({
      params: {
        query: {
          ...(args.accountId ? { accountId: args.accountId } : {}),
        },
      },
    }),
  }),
});
applyAuthBarrier(changeAccountMutation);

export const accountOptionsQuery = createQuery({
  ...createApiEffectWithContract('get', '/api/v1/account/options'),
  mapData: (data) => data.result,
  initialData: {
    publicLists: [],
    mainActivities: [],
  },
});
update(accountQuery, {
  on: changeAccountMutation,
  by: {
    success: ({ mutation }) => ({
      result: mutation.result,
    }),
  },
});

sample({
  clock: [addAccountMutation.finished.success, updateAccountMutation.finished.success],
  fn: () => ({}),
  target: viewerQuery.start,
});
export const $publicLists = combine(
  accountQuery.$data,
  accountOptionsQuery.$data,
  (accountData, accountOptionsData) => {
    return accountData?.options?.publicLists?.length
      ? accountData.options.publicLists
      : (accountOptionsData?.publicLists ?? []);
  },
);
export const $mainActivity = combine(
  accountQuery.$data,
  accountOptionsQuery.$data,
  (accountData, accountOptionsData) => {
    return accountData?.options?.mainActivities?.length
      ? accountData.options.mainActivities
      : (accountOptionsData?.mainActivities ?? []);
  },
);
sample({
  clock: [
    addAccountMutation.finished.success,
    updateAccountMutation.finished.success,
    updateBrandMutation.finished.success,
  ],
  fn: () => ({}),
  target: routes.profile.account.open,
});

const digitOnly = /^[0-9]+$/;

const BaseSchema = z.object({
  type: z.string().optional(),
  inn: z.string().max(12).min(10).regex(digitOnly),
  // checkedRules: z.literal(true),
  address: z.string().optional(),
});

const commonDefaults = {
  inn: '',
  // checkedRules: false,
  type: '',
};

const publicDefaults = {
  ...commonDefaults,
  fullName: '',
  shortName: '',
  kpp: '',
  ogrn: '',
  address: '',
};

const privateDefaults = sample({
  source: combine($firstName, $lastName),
  fn: ([firstName, lastName]) => ({
    ...commonDefaults,
    firstName,
    middleName: '',
    lastName,
  }),
});

const DynamicAccountForm = (accountKind: 'public' | 'private') => {
  const commonFields =
    accountKind === 'public'
      ? {
          fullName: z.string().nonempty(),
          shortName: z.string().nonempty(),
          kpp: z.string().min(9).max(9).nonempty().regex(digitOnly),
          ogrn: z.string().max(13).nonempty().regex(digitOnly),
        }
      : {
          firstName: z.string().nonempty(),
          middleName: z.string().nonempty(),
          lastName: z.string().nonempty(),
        };

  const schema = BaseSchema.extend(commonFields as any);
  const defaultValuesStore = accountKind === 'public' ? createStore(publicDefaults) : privateDefaults;

  return createForm({
    defaultValues: defaultValuesStore,
    validators: {
      onChange: schema,
    },
  });
};

export const addPublicAccountForm = DynamicAccountForm('public');
export const addPrivateAccountForm = DynamicAccountForm('private');
// Reset forms
{
  sample({
    clock: [routes.profile.account.updated, routes.profile.account.opened],
    source: addPublicAccountForm.$form,
    target: createEffect((form: any) => form.reset()),
  });
  sample({
    clock: [routes.profile.account.updated, routes.profile.account.opened],
    source: addPrivateAccountForm.$form,
    target: createEffect((form: any) => form.reset()),
  });
}
// Brand
const UpdateBrandDto = z.object({
  publicName: z.string().trim().min(1),
  description: z.string().nullable().optional(),
  mainActivityIds: z.array(z.string().uuid()).nullable().optional(),
  website: z
    .string()
    .url()
    .nullable()
    .optional()
    .or(z.literal(''))
    .transform((val) => (val === '' ? null : val)),
  email: z
    .string()
    .email()
    .nullable()
    .optional()
    .or(z.literal(''))
    .transform((val) => (val === '' ? null : val)),
  phoneNumber: z
    .string()
    .regex(/^\+?[0-9\s\-()]{7,20}$/, 'Некорректный номер телефона')
    .nullable()
    .optional()
    .or(z.literal(''))
    .transform((val) => (val === '' ? null : val)),
  publicListIds: z.array(z.string().uuid()).nullable().optional(),
});

export const updateBrandForm = createForm<Partial<UpdateBrandDto>>({
  defaultValues: accountQuery.$data.map((p) => ({
    publicName: p.brand?.publicName ?? '',
    description: p.brand?.description ?? '',
    mainActivityIds: p.brand?.mainActivities?.map((q) => q.id).filter((id): id is string => Boolean(id)) ?? [],
    website: p.brand?.website ?? '',
    email: p.brand?.email ?? '',
    phoneNumber: p.brand?.phoneNumber ?? '',
    publicListIds: p.brand?.publicLists?.map((q) => q.id).filter((id): id is string => Boolean(id)) ?? [],
  })),
  validators: {
    onChange: UpdateBrandDto,
  },
});
export const $hasAnyValueBrand = updateBrandForm.$values.map((values) =>
  Object.values(values ?? {}).some((value) => {
    if (Array.isArray(value)) return value.length > 0;
    return value !== undefined && value !== null && value !== '';
  }),
);
submit(updateBrandMutation, {
  on: updateBrandForm,
});
updateErrors(updateBrandForm, {
  on: addAccountMutation,
  fn: (args) => {
    const baseErrors = mapValidationErrorToForm(args);
    // Ранний выход
    if (!baseErrors) {
      return { errors: {} };
    }
    const { ...errors } = baseErrors.errors;
    errors.login = Object.entries(errors).map(
      ([message]) => `server: ${Array.isArray(message) ? message.join(', ') : message}`,
    );
    return { errors };
  },
});
updateErrors(updateBrandForm, {
  on: updateBrandMutation,
  fn: (args) => {
    const baseErrors = mapValidationErrorToForm(args);
    // Ранний выход
    if (!baseErrors) {
      return { errors: {} };
    }
    const { ...errors } = baseErrors.errors;
    errors.login = Object.entries(errors).map(
      ([message]) => `server: ${Array.isArray(message) ? message.join(', ') : message}`,
    );
    return { errors };
  },
});

export const fetchSuggestionsRaw = createEvent<string>();

export const fetchSuggestions = debounce({
  source: fetchSuggestionsRaw,
  timeout: 300,
});

export const $suggestions = createStore<components['schemas']['GetCompanyOut'][]>([]);

sample({
  source: searchInnMutation.finished.success,
  fn: ({ result }: { result: components['schemas']['GetCompanyOut'][] }) => result,
  target: $suggestions,
});

sample({
  source: fetchSuggestions,
  fn: (query: any) => ({ query }),
  target: searchInnMutation.start,
});
export const setSelectedAccount = createEvent<'publicForm' | 'privateForm'>();
export const resetSelectedAccount = createEvent<void>();
export const clearIsNew = createEvent();
export const clearResolvedType = createEvent();
export const $isNew = createStore<boolean | null>(false).on(clearIsNew, () => null);

export const $selectedAccount = createStore<'publicForm' | 'privateForm' | null>(null)
  .on(setSelectedAccount, (_, value) => value)
  .reset(resetSelectedAccount);

export const $resolvedType = combine(
  {
    data: accountQuery.$data,
    selected: $selectedAccount,
  },
  ({ data, selected }): 'public' | 'private' | 'publicForm' | 'privateForm' | null => {
    if (selected) return selected;
    switch (data?.type) {
      case 'SelfEmployed':
      case 'IndividualEntrepreneur':
        return 'private';
      case 'LegalEntity':
        return 'public';
      default:
        return null;
    }
  },
);
sample({
  clock: setSelectedAccount,
  target: resetSelectedAccount,
});

sample({
  clock: setSelectedAccount,
  target: $selectedAccount,
});

sample({
  clock: changeAccountMutation.finished.success,
  target: [clearIsNew, resetSelectedAccount],
});
sample({
  clock: [routes.profile.account.updated, routes.profile.account.opened],
  target: accountQuery.start,
});
sample({
  clock: [routes.profile.account.updated, routes.profile.account.opened],
  target: resetSelectedAccount,
});
sample({
  clock: accountQuery.finished.failure,
  filter: $isNew.map((isNew) => isNew !== true),
  fn: () => true,
  target: $isNew,
});

querySync({
  source: { new: $isNew },
  controls,
});

export const inviteUserMutation = createMutation({
  ...createApiEffectWithContract('post', '/api/v1/account/invitation/invite', {
    mapParams: (args: { email: string; role: 'Manager' | 'Developer' | 'Member' }) => ({
      params: {
        query: {
          email: args.email,
          role: args.role,
        },
      },
    }),
  }),
});

applyAuthBarrier(inviteUserMutation);
export const deleteUserMutation = createMutation({
  ...createApiEffectWithContract('delete', '/api/v1/account/invitation/remove', {
    mapParams: (args: { invitationId: string }) => ({
      params: {
        query: {
          invitationId: args.invitationId,
        },
      },
    }),
  }),
});

applyAuthBarrier(deleteUserMutation);
export const updateRoleUsersMutation = createMutation({
  ...createApiEffectWithContract('put', '/api/v1/account/invitation/update-roles', {
    mapParams: (
      args: components['schemas']['AccountRoleUpdateItem'] | components['schemas']['AccountRoleUpdateItem'][],
    ) => {
      return { body: Array.isArray(args) ? args : [args] };
    },
  }),
});

applyAuthBarrier(updateRoleUsersMutation);
const InviteUserIn = z.object({
  email: z.string().email('Введите корректную почту'),
  role: z.string().nonempty(),
});

export const inviteUserForm = createForm({
  defaultValues: {
    email: '',
    role: '',
  },
  validators: {
    onSubmit: InviteUserIn,
  },
});
submit(inviteUserMutation, {
  on: inviteUserForm,
});

sample({
  clock: merge([
    inviteUserMutation.finished.success,
    deleteUserMutation.finished.success,
    updateRoleUsersMutation.finished.success,
  ]),
  fn: () => ({}),
  target: accountQuery.start,
});
export const openInviteModal = createEvent();
export const closeInviteModal = createEvent();
export const $inviteModal = createStore(false)
  .on(openInviteModal, () => true)
  .on(closeInviteModal, () => false);

sample({
  clock: inviteUserMutation.finished.success,
  source: inviteUserForm.$form,
  target: [closeInviteModal, createEffect((form: any) => form.reset())],
});

sample({
  clock: addPrivateAccountForm.onSubmit,
  source: {
    addValues: addPrivateAccountForm.$values,
    brandValues: updateBrandForm.$values,
    isNew: $isNew,
  },
  fn: ({ addValues, brandValues }) => {
    return {
      ...addValues,
      brand: {
        ...(brandValues ?? {}),
      },
    } as any;
  },
  target: addAccountMutation.start,
});
sample({
  clock: addPublicAccountForm.onSubmit,
  source: {
    addValues: addPublicAccountForm.$values,
    brandValues: updateBrandForm.$values,
    isNew: $isNew,
  },
  fn: ({ addValues, brandValues }) => {
    return {
      ...addValues,
      brand: {
        ...(brandValues ?? {}),
      },
    } as any;
  },
  target: addAccountMutation.start,
});
export const $activeAccountId = createStore<string | null>(null);
export const updateAccountEvent = createEvent<{ formType: 'public' | 'private' }>();

sample({
  clock: updateAccountEvent,
  source: {
    publicValues: addPublicAccountForm.$values,
    privateValues: addPrivateAccountForm.$values,
    accountId: $activeAccountId,
  },
  fn: ({ publicValues, privateValues, accountId }, { formType }) => {
    const addValues = formType === 'public' ? publicValues : privateValues;
    return {
      accountId: accountId!,
      ...addValues,
    } as any;
  },
  target: updateAccountMutation.start,
});

export const setSelectedFile = createEvent<File | null>();

export const $selectedFile = createStore<File | null>(null).on(setSelectedFile, (_, file) => file);

export const $previewUrl = $selectedFile.map((file) => {
  if (!file) return null;
  return URL.createObjectURL(file);
});

sample({
  clock: [addAccountMutation.finished.success, updateBrandMutation.finished.success],
  source: $selectedFile,
  filter: (file): file is File => file !== null,
  target: avatarUploadMutation.start,
});

export const $activeAccountAvatar = createStore<string | null>(null);
sample({
  source: $viewer,
  fn: (q) => q?.activeAccountId ?? null,
  target: $activeAccountId,
});

sample({
  source: changeAccountMutation.finished.success.map(({ result }) => result?.id ?? null),
  target: $activeAccountId,
});
sample({
  source: $viewer,
  fn: (q) => {
    if (!q) return null;
    const activeAccount = q.accounts?.find((account) => account.id === q.activeAccountId);
    return activeAccount?.brand?.avatar.url ?? null;
  },
  target: $activeAccountAvatar,
});

sample({
  clock: changeAccountMutation.finished.success,
  fn: ({ result }) => result?.brand?.avatar.url ?? null,
  target: $activeAccountAvatar,
});
sample({
  clock: addAccountMutation.finished.success,
  fn: () => {
    return {
      severity: 'success',
      life: 1000,
      detail: 'Для публикации продуктов необходимо получить статус\nразработчика',
      summary: 'Данные об аккаунте отправлены на проверку',
    } as const;
  },
  target: showToast,
});
export const updateIsDeveloperMutation = createMutation({
  ...createApiEffectWithContract('patch', '/api/v1/account/{accountId}/developer', {
    mapParams: (args: { accountId: string; isDeveloper: boolean }) => ({
      params: {
        path: { accountId: args.accountId },
        query: { isDeveloper: args.isDeveloper },
      },
    }),
  }),
});
applyAuthBarrier(updateIsDeveloperMutation);
sample({
  clock: updateIsDeveloperMutation.finished.success,
  target: routes.profile.account.open,
});
sample({
  clock: updateIsDeveloperMutation.finished.success,
  fn: () => {},
  target: checkViewerQuery.start,
});

export const setIsSubmitted = createEvent<boolean>();
export const $isSubmitted = createStore(false).on(setIsSubmitted, (_, value) => value);
sample({
  clock: updateIsDeveloperMutation.finished.success,
  fn: () => {
    return {
      severity: 'success',
      life: 3000,
      summary: 'Статус аккаунта сменён на разработчик',
    } as const;
  },
  target: showToast,
});

sample({
  clock: changeAccountMutation.finished.success,
  fn: () => {},
  target: viewerQuery.start,
});
