import { useUnit } from 'effector-react';
import { EmptyProducts } from './ui/empty-products';
import { $viewer } from '~/shared/auth';
import { EmptyAccount } from './ui/empty-account';
import { $isVisibleModal, setIsVisibleModal, $productsList, productsQuery, deleteProductMutation } from './model';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { components } from '~/shared/api/schema';
import { Avatar } from 'primereact/avatar';
import { useRef, useState } from 'react';
import { TabPanel, TabView } from 'primereact/tabview';
import { Button } from 'primereact/button';
import { AddProductDialog } from './ui/product-dialog';
import { ProgressSpinner } from 'primereact/progressspinner';
import { Menu } from 'primereact/menu';
import { productAccountQuery } from '~/widgets/product-form/model';
import { SvgIcon } from '~/shared/ui/components/svg-icon';
import { Tooltip } from 'primereact/tooltip';
import clsx from 'clsx';
import { Dialog } from 'primereact/dialog';
import { Divider } from 'primereact/divider';
export type Products = components['schemas']['GetProductOut'][];
export function ProductsView() {
  const accountId = useUnit($viewer.map((viewer) => viewer?.activeAccountId));
  const hasDevStatus = useUnit($viewer.map((viewer) => viewer?.isDevStatus));
  const [products] = useUnit([$productsList]);
  const isLoading = useUnit(productsQuery.$pending);
  const [isVisibleModal, setIsVisibleModalCall] = useUnit([$isVisibleModal, setIsVisibleModal]);

  if (isLoading) {
    return (
      <div className="flex justify-content-center align-items-center" style={{ height: '300px' }}>
        <ProgressSpinner />
      </div>
    );
  }
  return (
    <>
      <div className="flex justify-content-between">
        <h1 className="m-0 text-4xl w-max">Продукты</h1>
        {accountId && hasDevStatus && products.length > 0 && (
          <Button
            className="h-2rem w-10rem"
            label="Добавить продукт"
            onClick={(event) => {
              event.preventDefault();
              setIsVisibleModalCall(true);
            }}
          />
        )}
        <AddProductDialog isVisibleModal={isVisibleModal} onHide={() => setIsVisibleModalCall(false)} />
      </div>
      <article className="flex flex-column mb-8">
        {!accountId || !hasDevStatus ? (
          <EmptyAccount />
        ) : products.length > 0 ? (
          <Products products={products} />
        ) : (
          <EmptyProducts />
        )}
      </article>
    </>
  );
}

const Products = ({ products }: { products: Products }) => {
  type StatusGroup = 'published' | 'unpublished' | 'archived';
  const STATUS_GROUPS: Record<StatusGroup, string[]> = {
    published: ['Approved', 'DevApproval'],
    unpublished: ['Draft', 'Pending', 'Rejected'],
    archived: ['Archived', 'Violation'],
  };
  const groups: Record<StatusGroup, Products> = {
    published: [],
    unpublished: [],
    archived: [],
  };
  for (const p of products) {
    const s = p.status;
    if (STATUS_GROUPS.published.includes(s)) groups.published.push(p);
    else if (STATUS_GROUPS.unpublished.includes(s)) groups.unpublished.push(p);
    else if (STATUS_GROUPS.archived.includes(s)) groups.archived.push(p);
  }
  const items = [
    {
      id: 'published',
      title: 'Опубликованные',
      description: 'опубликованных',
    },
    {
      id: 'unpublished',
      title: 'Неопубликованные',
      description: 'неопубликованных',
    },
    {
      id: 'archived',
      title: 'Снятые с публикации',
      description: 'снятых с публикации',
    },
  ];
  const renderPanel = (header: string, list: Products) => (
    <TabPanel header={items.find((item) => item.id === header)?.title}>
      {list.length > 0 ? (
        <ProductsTable products={list} />
      ) : (
        <div className="flex flex-column align-items-center justify-content-center" style={{ marginTop: '250px' }}>
          <span className="icon-10xl icon-cube surface-400 mb-4" />
          <span className="text-sm text-400">{`Нет ${items.find((item) => item.id === header)?.description} продуктов`}</span>
        </div>
      )}
    </TabPanel>
  );

  return (
    <TabView activeIndex={groups.published.length > 0 ? 0 : 1} className="mt-3">
      {renderPanel('published', groups.published)}
      {renderPanel('unpublished', groups.unpublished)}
      {renderPanel('archived', groups.archived)}
    </TabView>
  );
};

const ProductsTable = ({ products }: { products: Products }) => {
  const [isVisibleModal, setIsVisibleModalCall] = useState(false);
  const menuRef = useRef<Menu>(null);
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null);
  const onMenuToggle = (event: React.MouseEvent, productId: string) => {
    setSelectedProductId(productId);
    menuRef.current?.toggle(event);
  };
  const statusTextMap = {
    Draft: 'Черновик',
    Pending: 'На модерации',
    Rejected: 'Не прошел модерацию',
    DevApproval: 'Требует подтверждения разработчика',
    Approved: 'Подтвержден разработчиком',
    Violation: 'Нарушение правил платформы',
    Archived: 'В архиве',
  };
  return (
    <>
      <DataTable
        emptyMessage="Нет данных"
        className="pl-0"
        value={products}
        onRowClick={(e) => {
          const productId = e.data.id;
          productAccountQuery.start({ id: productId });
        }}
        rowClassName={() => 'hover:surface-100'}
      >
        <Column
          field="name"
          header="Продукт"
          className="w-12rem"
          body={(row) => (
            <div className="flex flex-wrap align-items-center">
              <Avatar
                image={row.avatar}
                className="mr-3 border-1 bg-white"
                label={!row.avatar ? (row.name?.trim()?.slice(0, 2)?.toUpperCase() ?? '') : undefined}
              />
              <div className="flex flex-column">
                <span className="vertical-align-middle">{row.name}</span>
              </div>
            </div>
          )}
        />
        <Column field="version" className="w-1rem" header="Версия" />
        <Column
          field="status"
          className="w-1rem"
          header="Состояние"
          body={(row: components['schemas']['GetProductOut']) => (
            <>
              <div className="inline-block" id={`icon-tooltip-target-${row.id}`}>
                <SvgIcon name={`status-${row.status.toLowerCase()}`} />
              </div>
              <Tooltip
                target={`#icon-tooltip-target-${row.id}`}
                content={statusTextMap[row.status as keyof typeof statusTextMap] || ''}
                position="bottom"
              />
            </>
          )}
        />
        <Column
          field="createdAt"
          className="w-1rem text-right px-0"
          headerClassName="flex justify-content-end"
          header="Дата публикации"
        />
        <Column
          field="menu"
          className="w-6rem text-right"
          body={(rowData, { rowIndex }) => (
            <span
              className="icon-2xl icon-dots-vertical cursor-pointer"
              onClick={(event) => {
                event.stopPropagation();
                onMenuToggle(event, rowData.id);
              }}
            />
          )}
        />
      </DataTable>
      <Menu
        ref={menuRef}
        popup
        className="w-9rem text-color-secondary border-round-xl border-1 py-0"
        model={[
          {
            label: 'Опубликовать',
            disabled: true,
            template: (item, options) => (
              <div
                className={clsx('p-menuitem-content px-3 py-2', {
                  'p-disabled': item.disabled,
                })}
                style={{ borderTopLeftRadius: '12px', borderTopRightRadius: '12px' }}
                onClick={(e) => {
                  options.onClick?.(e);
                  if (selectedProductId) {
                    deleteProductMutation.start({ id: selectedProductId });
                  }
                }}
              >
                {item.label}
              </div>
            ),
          },

          {
            label: 'Удалить',
            template: (item, options) => (
              <div
                className="p-menuitem-exit items-center gap-2 text-red-600"
                onClick={(e) => {
                  options.onClick?.(e);
                  setIsVisibleModalCall(true);
                }}
              >
                {item.label}
              </div>
            ),
          },
        ]}
      />
      <Dialog
        header="Удаление продукта"
        headerClassName="pb-3 pt-4"
        className="text-sm"
        style={{ width: '512px' }}
        visible={isVisibleModal}
        onHide={() => setIsVisibleModalCall(false)}
      >
        <section className="flex flex-column gap-1 mb-6">
          <span className="text-color-secondary text-sm cursor-pointer">
            Вы точно хотите удалить продукт? После удаления его будет
            <br />
            невозможно восстановить
          </span>
        </section>
        <div className="flex justify-content-between mb-4">
          <Button
            label="Отменить"
            severity="secondary"
            type="reset"
            onClick={(event) => {
              event.preventDefault();
              setIsVisibleModalCall(false);
            }}
            className="w-13rem"
          />
          <Button
            label="Удалить"
            className="w-13rem"
            type="submit"
            onClick={(event) => {
              event.preventDefault();
              if (selectedProductId) {
                deleteProductMutation.start({ id: selectedProductId });
              }
              setIsVisibleModalCall(false);
            }}
          />
        </div>
      </Dialog>
    </>
  );
};
