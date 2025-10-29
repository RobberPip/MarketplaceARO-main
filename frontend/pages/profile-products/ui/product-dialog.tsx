import { Dropdown } from '~/shared/ui/dropdown';
import { Dialog } from 'primereact/dialog';
import { Divider } from 'primereact/divider';
import { InputText } from 'primereact/inputtext';
import { useUnit } from 'effector-react';
import { Button } from 'primereact/button';
import { productForm, addDraftEvent, productsOptionsQuery } from '~/widgets/product-form/model';
import { $product, setProductName, setSelectedType } from '../model';

export const AddProductDialog = ({
  isVisibleModal,
  onHide,
}: {
  isVisibleModal: boolean;
  onHide: () => void;
}) => {
  const { form } = useUnit(productForm);
  const [productName, setProductNameCall] = useUnit([$product.map((v) => v.name), setProductName]);
  const [selectedType, setSelectedTypeCall] = useUnit([$product.map((v) => v.type), setSelectedType]);
  const isFormValid = selectedType && productName.trim() !== '';
  const productTypes = useUnit(productsOptionsQuery.$data.map((q) => q?.productTypes ?? []));
  return (
    <Dialog
      header="Добавление продукта"
      headerClassName="pb-3 pt-4"
      className="text-sm"
      style={{ width: '512px' }}
      visible={isVisibleModal}
      onHide={onHide}
    >
      <section className="flex flex-column gap-1 mb-3">
        <label className="text-color-secondary text-xs">Класс продукта*</label>
        <Dropdown
          placeholder="Выберите из списка"
          optionLabel="combinedName"
          options={productTypes ?? []}
          value={selectedType ?? null}
          onChange={(e) => setSelectedTypeCall(e.value)}
        />
        <span className="text-color-secondary text-xs cursor-pointer">
          Выберите категорию продукта из предложенного списка, чтобы определить тип решения
        </span>
      </section>
      <section className="flex flex-column gap-1">
        <label className="text-color-secondary text-xs">Наименование продукта*</label>
        <InputText placeholder="Введите" value={productName} onChange={(e) => setProductNameCall(e.target.value)} />
      </section>
      <Divider className="my-4" />
      <div className="flex justify-content-between mb-4">
        <Button
          label="Отменить"
          severity="secondary"
          type="reset"
          onClick={(event) => {
            event.preventDefault();
            onHide();
            setSelectedTypeCall({});
            setProductNameCall('');
          }}
          className="w-13rem"
        />
        <Button
          label="Добавить"
          className="w-13rem"
          type="submit"
          onClick={(event) => {
            event.preventDefault();
            form.setFieldValue('name', productName);
            form.setFieldValue('typeId', selectedType?.id ?? '');
            addDraftEvent({ name: productName, type: selectedType.id });
            setSelectedTypeCall({});
            setProductNameCall('');
            onHide();
          }}
          disabled={!isFormValid}
        />
      </div>
    </Dialog>
  );
};
