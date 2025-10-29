import { useUnit } from 'effector-react';
import { useField } from '@tanstack/react-form';
import { Button } from 'primereact/button';
import { $activeView, $itemsProducts, ProductIn, productForm, updateDraftEvent } from './model';
import { BreadCrumb } from 'primereact/breadcrumb';
import type { MenuItem } from 'primereact/menuitem';
import type { MenuItemOptions } from 'primereact/menuitem';
import { routes } from '~/shared/routing';
import { Link } from 'atomic-router-react';
import { GeneralInformation } from './ui/general-info';

export const ProductFormView = () => {
  const { extendedForm: form } = useUnit(productForm);
  const ActiveView = useUnit($activeView);
  const iconItemTemplate = (item: MenuItem, options: MenuItemOptions) => {
    return (
      <a className={options.className}>
        <span className="mx-2">/</span>
        {item.label ?? ''}
      </a>
    );
  };
  const items = useUnit($itemsProducts);
  const itemsProducts: MenuItem[] = items.map((label) => ({ label, template: iconItemTemplate }));
  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        updateDraftEvent();
      }}
    >
      <BreadCrumb
        className="mb-3 border-none p-0 text-color-secondary"
        home={{
          label: 'Продукты',
          template: (item, options) => (
            <Link className={`${options.className} text-color-secondary`} to={routes.profile.products}>
              {item.label}
            </Link>
          ),
        }}
        model={itemsProducts}
        pt={{
          separator: { className: 'hidden' },
          root: { style: { background: 'none' } },
        }}
      />
      <div className="flex gap-2 w-full justify-between mb-5">
        <form.Field name="name" children={(field) => <h2 className="my-0">{field.state.value}</h2>} />
      </div>
      {ActiveView ? <ActiveView /> : <GeneralInformation />}
      {/* {currentStep === 1 && }
      {/* {currentStep === 2 && <Step2 />}
      {currentStep === 3 && <Step3 />} */}
      <form.Subscribe
        selector={(state) => ({
          canSubmit: state.canSubmit,
        })}
      >
        {({ canSubmit }) => (
          <Button type="submit" className="mt-2 mb-8" label="Сохранить" severity="secondary" disabled={!canSubmit} />
        )}
      </form.Subscribe>
    </form>
  );
};

export const useDataField = (name: keyof ProductIn['data']) => {
  const form = useUnit(productForm.$extendedForm);
  if (!form) {
    throw new Error('Form is not defined');
  }
  // biome-ignore lint/suspicious/noExplicitAny: <explanation>
  return useField<any, any>({ form, name: `data.${name}` });
};
