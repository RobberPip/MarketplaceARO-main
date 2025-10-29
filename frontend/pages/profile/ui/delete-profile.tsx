import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { useState } from 'react';

// TODO future block finance
export const DeleteProfile = () => {
  const [deleteProfileModal, setDeleteProfileModal] = useState<boolean>(false);
  return (
    <>
      <Button
        label="Удалить профиль"
        className="w-10rem"
        severity="danger"
        onClick={() => setDeleteProfileModal(true)}
      />
      <Dialog
        header="Удаление профиля"
        headerClassName="pb-3"
        className="text-sm line-height-2"
        visible={deleteProfileModal}
        style={{ width: '512px' }}
        onHide={() => {
          if (!deleteProfileModal) return;
          setDeleteProfileModal(false);
        }}
      >
        <p>Вместе с профилем удалится вся информация о вас, сохранённые продукты, история операций и т.д.....</p>
        <p>
          Вам больше не будут выставляться счета, и через 90 дней ваше имя пользователя будет доступно любому
          пользователю IT market.
        </p>
        <p className="mb-0">
          Для получения дополнительной информации прочтите нашу статью{' '}
          <a className="text-primary" href="#">
            "Удаление профиля пользователя"
          </a>
          .
        </p>
        <div className="flex justify-content-between mt-4 mb-4">
          <Button
            label="Отменить"
            onClick={() => setDeleteProfileModal(false)}
            severity="secondary"
            className="w-13rem"
          />
          <Button label="Продолжить" className="w-13rem" />
        </div>
      </Dialog>
    </>
  );
};
