import { Button } from 'primereact/button';
import { Divider } from 'primereact/divider';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { BrandAccount } from './brand';
import { useUnit } from 'effector-react';
import {
  $isSubmitted,
  $suggestions,
  accountOptionsQuery,
  accountQuery,
  addAccountMutation,
  addPublicAccountForm,
  fetchSuggestionsRaw,
  setIsSubmitted,
  updateAccountEvent,
} from '../model';
import { AutoComplete, AutoCompleteChangeEvent } from 'primereact/autocomplete';
import { components } from '~/shared/api/schema';
import { StatusIcon, itemTemplate } from '..';
import { useState } from 'react';
import { Dialog } from 'primereact/dialog';
import { DeleteAccount } from './delete';
import { DeveloperAccount } from './developer';
import { InvitionUsers } from './Invition-users';
import { ErrorAccount } from './error';

export function PublicAccount() {
  const accountData = useUnit(accountQuery.$data);
  const [isEditing, setIsEditing] = useState(false);

  if (accountData.status === 'Pending') {
    setIsSubmitted(true);
  } else {
    setIsSubmitted(false);
  }

  return (
    <article className="flex flex-column mb-4">
      <h1 className="m-0 mb-3 text-2xl font-medium">
        {accountData.fullName}
        <StatusIcon status={accountData.status} />
      </h1>

      <span className="text-sm mb-4 text-color-secondary">
        Юридическое лицо {' | '}
        {accountData.status === 'Approved'
          ? 'Подтверждён'
          : accountData.status === 'Blocked'
            ? 'Отклонён'
            : 'На подтверждении'}
        {accountData.isDeveloper ? ' | Разработчик' : ''}
      </span>

      {accountData.status === 'Blocked' ? (
        <>
          {!isEditing ? (
            <ErrorAccount onEdit={() => setIsEditing(true)} />
          ) : (
            <>
              <h2 className="m-0 text-lg font-medium">Юридические данные</h2>
              <Divider className="mt-2 mb-3" />
              <PublicAccountForm />
              <BrandAccount publicAccount={true} disabled={!accountData.id} />
            </>
          )}
        </>
      ) : (
        <>
          <h2 className="m-0 text-lg font-medium">Юридические данные</h2>
          <Divider className="mt-2 mb-3" />

          {accountData.status === 'Approved' ? (
            <>
              <span className="text-sm mb-2">ИНН {accountData.inn}</span>
              <span className="text-sm mb-2">КПП {accountData.kpp}</span>
              {accountData.address && <span className="text-sm mb-2">{accountData.address}</span>}
              <span className="text-xs mb-4 text-color-secondary">
                Для изменения юридических данных напишите в поддержку на почту support@arogroup.ru
              </span>
            </>
          ) : (
            <PublicAccountForm accountData={accountData} />
          )}

          <BrandAccount publicAccount={true} disabled={!accountData.id} />
          {accountData.status === 'Approved' &&
            accountData.brand?.status === 'Approved' &&
            accountData.userRole === 'Owner' && (
              <>
                <div className="mt-4">
                  <InvitionUsers accountData={accountData} />
                </div>
                {!accountData.isDeveloper && <DeveloperAccount />}
                <DeleteAccount />
              </>
            )}
        </>
      )}
    </article>
  );
}

export function NewPublicAccount() {
  const isSubmitted = useUnit($isSubmitted);
  return (
    <>
      <h1 className="m-0 text-2xl font-medium">Аккаунт</h1>
      <h2 className="m-0 text-lg font-medium">Юридические данные</h2>
      <Divider className="mt-2 mb-3" />
      <span className="mt-2 text-xs text-color-secondary">
        После создания и подтверждения аккаунта поля перестанут быть доступными для
        <br /> редактирования. Укажите верные данные заранее
      </span>
      <PublicAccountForm isNew={true} />
      <BrandAccount publicAccount={true} disabled={!isSubmitted} new={true} />
    </>
  );
}
export function PublicAccountForm({
  accountData = null,
  isNew = false,
}: {
  accountData?: components['schemas']['GetAccountOut'] | null;
  isNew?: boolean;
}) {
  const { extendedForm: form } = useUnit(addPublicAccountForm);
  const { pending } = useUnit(addAccountMutation);
  const [suggestions, fetch] = useUnit([$suggestions, fetchSuggestionsRaw]);
  const [modalWindow, setModalWindow] = useState<boolean>(false);
  const isSubmitted = useUnit($isSubmitted);
  const updateIsSubmitted = useUnit(setIsSubmitted);
  const handleCompanySelect = (company: components['schemas']['GetCompanyOut']) => {
    const entries = [
      ['address', company.fullAddress || ''],
      ['fullName', company.fullName || ''],
      ['ogrn', company.ogrn || ''],
      ['shortName', company.shortName || ''],
    ] as const;

    for (const [field, value] of entries) {
      form.setFieldValue(field, value);
      form.setFieldMeta(field, (prev) => ({ ...prev, touched: true, isDirty: true }));
      form.validateField(field, 'change');
    }
  };

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        form.handleSubmit();
      }}
      onKeyDown={(e) => {
        if (e.code === 'Enter') {
          e.preventDefault();
        }
      }}
    >
      <section className="mb-5 flex flex-column text-color-secondary">
        <div className="flex gap-5">
          <form.Field
            name="inn"
            children={(field) => (
              <label className="flex w-13rem text-xs flex-column my-2">
                ИНН*
                <AutoComplete
                  className="mt-1"
                  inputClassName="w-full"
                  id={field.name}
                  disabled={isSubmitted}
                  name={field.name}
                  value={accountData?.inn ?? field.state.value ?? ''}
                  onBlur={field.handleBlur}
                  suggestions={suggestions}
                  completeMethod={(e) => fetch(e.query)}
                  onChange={(e: AutoCompleteChangeEvent) => {
                    const value = e.value as components['schemas']['GetCompanyOut'] | string;
                    if (typeof value === 'object' && value !== null) {
                      field.handleChange(value.inn || '');
                      handleCompanySelect(value);
                    } else {
                      field.handleChange(value || '');
                    }
                  }}
                  itemTemplate={itemTemplate}
                  placeholder="Введите"
                  onKeyPress={(e) => {
                    const isDigit = /[0-9]/.test(e.key);
                    if (!isDigit) {
                      e.preventDefault();
                    }
                  }}
                />
              </label>
            )}
          />
          <form.Field
            name="kpp"
            children={(field) => (
              <label className="flex w-13rem text-xs flex-column my-2">
                КПП*
                <InputText
                  className="mt-1"
                  id={field.name}
                  disabled={isSubmitted}
                  name={field.name}
                  value={accountData?.kpp ?? field.state.value ?? ''}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                  placeholder="Введите"
                />
              </label>
            )}
          />
        </div>
        <form.Field
          name="ogrn"
          children={(field) => (
            <label className="flex w-13rem text-xs flex-column">
              ОГРН*
              <InputText
                className="mt-1"
                id={field.name}
                disabled={isSubmitted}
                name={field.name}
                value={accountData?.ogrn ?? field.state.value ?? ''}
                onBlur={field.handleBlur}
                onChange={(e) => field.handleChange(e.target.value)}
                placeholder="Введите"
              />
            </label>
          )}
        />
        <form.Field
          name="fullName"
          children={(field) => (
            <label className="flex w-28rem text-xs flex-column mt-2">
              Полное имя*
              <InputText
                className=" mt-1"
                id={field.name}
                disabled={isSubmitted}
                name={field.name}
                value={accountData?.fullName ?? field.state.value ?? ''}
                onBlur={field.handleBlur}
                onChange={(e) => field.handleChange(e.target.value)}
                placeholder="Введите"
              />
            </label>
          )}
        />
        <form.Field
          name="shortName"
          children={(field) => (
            <label className="flex w-28rem text-xs flex-column mt-2">
              Короткое имя*
              <InputText
                className=" mt-1"
                id={field.name}
                disabled={isSubmitted}
                name={field.name}
                value={accountData?.shortName ?? field.state.value ?? ''}
                onBlur={field.handleBlur}
                onChange={(e) => field.handleChange(e.target.value)}
                placeholder="Введите"
              />
            </label>
          )}
        />
        <form.Field
          name="address"
          children={(field) => (
            <label className="mt-2 text-xs" style={{ marginRight: '224px' }}>
              Юридический адрес*
              <InputTextarea
                rows={3}
                disabled={isSubmitted}
                id={field.name}
                name={field.name}
                value={accountData?.address ?? field.state.value ?? ''}
                onBlur={field.handleBlur}
                onChange={(e) => field.handleChange(e.target.value)}
                className="mt-2 w-full"
                placeholder="Введите"
              />
            </label>
          )}
        />
        {/* <form.Field
            name="checkedRules"
            children={(field) => (
              <div className="flex gap-3 mb-4 mt-3">
                <Checkbox
                  inputId="rules"
                  id={field.name}
                  name={field.name}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(!!e.target.checked)}
                  checked={Boolean(field.state.value)}
                  className="ml-1"
                />
                <label htmlFor="rules" className="text-primary text-xs">
                  Соглашаюсь с правилами взаимодействия на площадке
                </label>
              </div>
            )}
          /> */}
        <Dialog
          header="Модерация аккаунта"
          headerClassName="pb-3"
          className="text-sm"
          visible={modalWindow}
          style={{ width: '512px' }}
          onHide={() => {
            if (!modalWindow) return;
            setModalWindow(false);
          }}
        >
          <div className="flex flex-column">
            <span>
              Отправить ваш аккаунт на проверку? После её прохождения, некоторые данные аккаунта нельзя будет изменить.
            </span>
            <div className="flex justify-content-between mt-7">
              <Button
                type="button"
                severity="secondary"
                label="Отменить"
                className="mb-4 w-13rem"
                onClick={() => setModalWindow(false)}
              />
              <Button
                type="button"
                onClick={() => {
                  form.setFieldValue('type', 'LegalEntity');
                  setModalWindow(false);
                  if (isNew === false) {
                    updateAccountEvent({ formType: 'public' });
                  } else {
                    updateIsSubmitted(true);
                    accountOptionsQuery.start();
                  }
                }}
                loading={pending}
                label="Отправить"
                className="mb-4 w-13rem"
              />
            </div>
          </div>
        </Dialog>
        <form.Subscribe
          selector={(state) => ({
            canSubmit: state.isTouched && state.canSubmit,
          })}
        >
          {({ canSubmit }) => {
            return canSubmit ? (
              <Button
                type="button"
                label={isNew === false ? 'Сохранить' : 'Продолжить'}
                disabled={isSubmitted}
                onClick={() => setModalWindow(true)}
                className="mt-4"
              />
            ) : null;
          }}
        </form.Subscribe>
      </section>
    </form>
  );
}
