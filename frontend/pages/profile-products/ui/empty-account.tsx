import { Button } from 'primereact/button';
import { routes } from '~/shared/routing';

export const EmptyAccount = () => {
  return (
    <>
      <form style={{ marginTop: '190px' }} className="flex flex-column align-items-center text-center">
        <div className="w-max">
          <h2 className="mt-0 mb-4 text-2xl font-medium">Нет статуса разработчика</h2>
          <p className="text-color-secondary text-sm mb-5 line-height-3">
            Для добавления собственных программных продуктов в IT market
            <br />
            необходимо добавить аккаунт и приобрести статус разработчика
          </p>
          <Button
            label="Перейти в аккаунт"
            onClick={() => {
              routes.profile.account.open();
            }}
            className="w-10rem mt-3"
          />
        </div>
      </form>
    </>
  );
};
