import { Divider } from 'primereact/divider';

export function DeleteAccount() {
  return (
    <section className="flex flex-column">
      <h2 className="m-0 text-lg mt-5 font-medium">Удаление аккаунта</h2>
      <Divider className="mt-2 mb-3" />

      <span className="text-xs text-color-secondary mb-4">
        Удаление аккаунта приведёт к потере доступа к платформе IT market. Ваши личные данные также будут удалены
        <br />
        без возможности восстановления. Для удаления аккаунта обратитесь в поддержку на почту update@arogroup.ru
      </span>
    </section>
  );
}
