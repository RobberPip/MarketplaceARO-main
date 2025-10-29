import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { Divider } from 'primereact/divider';
import { Checkbox } from 'primereact/checkbox';
import { useState } from 'react';
import { Link } from 'atomic-router-react';
import { routes } from '~/shared/routing';
import { $activeAccountId, updateIsDeveloperMutation } from '../model';
import { useUnit } from 'effector-react';

export function DeveloperAccount() {
  const [modalWindow, setModalWindow] = useState<boolean>(false);
  const [agreementChecked, setAgreementChecked] = useState<boolean>(false);
  const accountId = useUnit($activeAccountId);
  const { pending } = useUnit(updateIsDeveloperMutation);
  return (
    <section className="flex flex-column">
      <h2 className="m-0 text-lg mt-5 font-medium">Статус разработчика</h2>
      <Divider className="mt-2 mb-3" />

      <span className="text-xs text-color-secondary mb-4">
        Приобретение статуса разработчика позволит загружать на IT market собственные программные продукты
      </span>
      <Button label="Стать разработчиком" onClick={() => setModalWindow(true)} severity="secondary" />

      <Dialog
        header="Статус разработчика"
        headerClassName="pb-3"
        className="text-sm"
        visible={modalWindow}
        style={{ width: '512px' }}
        onHide={() => setModalWindow(false)}
      >
        <div className="flex flex-column">
          <span>
            Для получения статуса разработчика вы должны согласиться
            <br /> с Соглашением о распространении Программных продуктов
          </span>

          <div className="flex align-items-center mt-4">
            <Checkbox
              inputId="agreement"
              checked={agreementChecked}
              onChange={(e) => setAgreementChecked(e.checked ?? false)}
            />
            <label htmlFor="agreement" className="ml-2 text-xs">
              Соглашаюсь с{' '}
              <Link
                to={routes.docs}
                target="_blank"
                query={{ name: 'user-agreement' }}
                className="underline text-primary"
              >
                Соглашением о распространении Программных продуктов
              </Link>
            </label>
          </div>
          <Divider className="mt-6" />
          <div className="flex justify-content-between my-4">
            <Button
              type="button"
              severity="secondary"
              label="Отменить"
              className="w-13rem"
              onClick={() => {
                setModalWindow(false);
                setAgreementChecked(false);
              }}
            />
            <Button
              type="button"
              label="Получить статус"
              className="w-13rem"
              loading={pending}
              disabled={!agreementChecked || !accountId}
              onClick={() => {
                if (accountId) {
                  updateIsDeveloperMutation.start({ accountId: accountId, isDeveloper: true });
                }
              }}
            />
          </div>
        </div>
      </Dialog>
    </section>
  );
}
