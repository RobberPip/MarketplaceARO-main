import { useUnit } from 'effector-react';
import { Button } from 'primereact/button';
import { accountQuery } from '../model';

type ErrorAccountProps = {
  onEdit: () => void;
};

export const ErrorAccount = ({ onEdit }: ErrorAccountProps) => {
  const accountData = useUnit(accountQuery.$data);
  return (
    <>
      <span className="text-sm mb-4">
        Ваш аккаунт не прошёл проверку. Скорректируйте информацию об аккаунте и сохраните
        <br /> заново, или напишите в поддержку на почту support@arogroup.ru, если вы не согласны
        <br /> с отклонением заявки.
      </span>
      {accountData.userRole === 'Owner' && <Button label="Редактировать" onClick={onEdit} />}
    </>
  );
};
