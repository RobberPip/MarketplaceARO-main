import { useUnit } from 'effector-react';
import { Button } from 'primereact/button';
import { Checkbox } from 'primereact/checkbox';
import { $checkedAgreements, updateCheckedAgreements } from '~/pages/profile/model';
import { AddProductDialog } from './product-dialog';
import { $isVisibleModal, setIsVisibleModal } from '../model';

export const EmptyProducts = () => {
  const [checkedAgreements, setCheckedAgreements] = useUnit([$checkedAgreements, updateCheckedAgreements]);
  const [isVisibleModal, setIsVisibleModalCall] = useUnit([$isVisibleModal, setIsVisibleModal]);

  return (
    <>
      <form style={{ marginTop: '190px' }} className="flex flex-column align-items-center text-center">
        <div className="w-max">
          <h2 className="mt-0 mb-4 text-2xl font-medium">Нет активных продуктов</h2>
          <p className="text-color-secondary mb-4 text-sm line-height-3">
            Добавьте продукты в реестр Arogroup, заполните необходимую
            <br />
            информацию, и помогайте людям находить решения для своих задач
          </p>
          <div className="flex">
            <Checkbox
              className="mr-3"
              onChange={(e) => setCheckedAgreements(!!e.checked)}
              checked={checkedAgreements}
            />
            <span className="text-color-secondary text-left mb-5">
              Я подтверждаю, что ознакомлен и согласен с{' '}
              <a href="#" className="text-color-secondary">
                Соглашением <br />о распространении программных продуктов
              </a>
            </span>
          </div>
          <Button
            label="Добавить продукт"
            onClick={(event) => {
              event.preventDefault();
              setIsVisibleModalCall(true);
            }}
            className="w-13rem mt-2"
            disabled={!checkedAgreements}
          />
          <AddProductDialog isVisibleModal={isVisibleModal} onHide={() => setIsVisibleModalCall(false)} />
        </div>
      </form>
    </>
  );
};
