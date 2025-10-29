import { Checkbox } from 'primereact/checkbox';
import { Divider } from 'primereact/divider';
import { Dropdown } from 'primereact/dropdown';
import { InputText } from 'primereact/inputtext';
import { BrandAccount } from './brand';
import { useUnit } from 'effector-react';
import { useState } from 'react';
import { InputTextarea } from 'primereact/inputtextarea';
import { Button } from 'primereact/button';
import {
  $isSubmitted,
  $suggestions,
  accountOptionsQuery,
  accountQuery,
  addAccountMutation,
  addPrivateAccountForm,
  fetchSuggestionsRaw,
  setIsSubmitted,
  updateAccountEvent,
  updateBrandForm,
  updateBrandMutation,
} from '../model';
import { AutoComplete, AutoCompleteChangeEvent } from 'primereact/autocomplete';
import { components } from '~/shared/api/schema';
import { StatusIcon, itemTemplate } from '..';
import { Dialog } from 'primereact/dialog';
import { DeleteAccount } from './delete';
import { DeveloperAccount } from './developer';
import { InvitionUsers } from './Invition-users';
import { ErrorAccount } from './error';

export function PrivateAccount() {
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
        {accountData.type?.toLowerCase() === 'individualentrepreneur' ? 'ИП ' : ''}
        {accountData.fullName}
        <StatusIcon status={accountData.status} />
      </h1>

      <span className="text-sm mb-4 text-color-secondary">
        {accountData.type?.toLowerCase() === 'individualentrepreneur'
          ? 'Индивидуальный предприниматель'
          : 'Самозанятый'}
        {' | '}
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
              <PrivateAccountForm />
              <BrandAccount publicAccount={false} disabled={!accountData.id} />
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
              {accountData.address && accountData.type?.toLowerCase() !== 'selfemployed' && (
                <span className="text-sm mb-2">{accountData.address}</span>
              )}
              <span className="text-xs mb-4 text-color-secondary">
                Для изменения имени и фамилии напишите в поддержку на почту support@arogroup.ru
              </span>
            </>
          ) : (
            <PrivateAccountForm accountData={accountData} />
          )}

          <BrandAccount publicAccount={false} disabled={!accountData.id} />

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

export function NewPrivateAccount() {
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
      <PrivateAccountForm isNew={true} />
      <BrandAccount publicAccount={false} disabled={!isSubmitted} new={true} />
    </>
  );
}
export function PrivateAccountForm({
  accountData = null,
  isNew = false,
}: {
  accountData?: components['schemas']['GetAccountOut'] | null;
  isNew?: boolean;
}) {
  const { extendedForm: form } = useUnit(addPrivateAccountForm);
  const { pending } = useUnit(addAccountMutation);
  const [selectedActivity, setSelectedActivity] = useState<{ code: string }>();
  const [suggestions, fetch] = useUnit([$suggestions, fetchSuggestionsRaw]);
  const [modalWindow, setModalWindow] = useState<boolean>(false);
  const isSubmitted = useUnit($isSubmitted);
  const updateIsSubmitted = useUnit(setIsSubmitted);
  const handleCompanySelect = (company: components['schemas']['GetCompanyOut']) => {
    form.setFieldValue('address', company.fullAddress || '');
  };
  const activityOptions = [
    { name: 'Индивидуальный предприниматель', code: 'individualEntrepreneur' },
    { name: 'Самозанятость', code: 'selfEmployed' },
  ];
  const getSelectedActivity = () => {
    if (selectedActivity) return selectedActivity;

    if (accountData?.type) {
      const normalizedType = accountData.type.toLowerCase();
      return activityOptions.find((opt) => opt.code.toLowerCase() === normalizedType) ?? null;
    }

    return null;
  };
  const currentActivityCode =
    selectedActivity?.code ??
    (accountData?.type ? accountData.type.charAt(0).toLowerCase() + accountData.type.slice(1) : undefined);
  function splitFullName(fullName?: string) {
    if (!fullName) return { firstName: '', lastName: '', middleName: '' };
    const [lastName = '', firstName = '', ...middleParts] = fullName.trim().split(' ');
    const middleName = middleParts.join(' ');
    return { firstName, lastName, middleName };
  }
  const { firstName, lastName, middleName } = splitFullName(accountData?.fullName ?? undefined);

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        form.setFieldValue('type', selectedActivity?.code ?? '');
        form.handleSubmit();
      }}
    >
      <section className=" mb-5 flex flex-column text-color-secondary">
        <label className="flex text-xs flex-column mt-2">
          Формат деятельности*
          <form.Field
            name="type"
            children={(field) => (
              <Dropdown
                className="w-28rem mt-1"
                options={activityOptions}
                value={getSelectedActivity()}
                disabled={isSubmitted}
                onChange={(e) => {
                  setSelectedActivity(e.value);
                  field.handleChange(e.value.code);
                }}
                optionLabel="name"
                placeholder="Выберите из списка"
              />
            )}
          />
        </label>
        <div className="flex gap-5 mt-3">
          <form.Field
            name="inn"
            children={(field) => (
              <label className="flex w-13rem text-xs flex-column">
                ИНН*
                <AutoComplete
                  className="mt-1"
                  inputClassName="w-full"
                  id={field.name}
                  name={field.name}
                  value={accountData?.inn ?? field.state.value ?? ''}
                  onBlur={field.handleBlur}
                  disabled={isSubmitted}
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
          <label className="flex w-13rem text-xs flex-column">
            КПП*
            <InputText className="mt-1" disabled={true} placeholder="Введите" />
          </label>
        </div>
        <label className="flex w-13rem text-xs flex-column my-3">
          ОГРН*
          <InputText className="mt-1" disabled={true} placeholder="Введите" />
        </label>
        <div className="flex gap-5">
          <form.Field
            name="firstName"
            children={(field) => (
              <label className="flex w-13rem text-xs flex-column">
                Имя*
                <InputText
                  className="mt-1"
                  id={field.name}
                  disabled={isSubmitted}
                  name={firstName ?? field.name}
                  value={firstName ? firstName : (field.state.value ?? '')}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                  placeholder="Введите"
                />
              </label>
            )}
          />
          <form.Field
            name="lastName"
            children={(field) => (
              <label className="flex w-13rem text-xs flex-column">
                Фамилия*
                <InputText
                  className="mt-1"
                  id={field.name}
                  disabled={isSubmitted}
                  name={field.name}
                  value={lastName ? lastName : (field.state.value ?? '')}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                  placeholder="Введите"
                />
              </label>
            )}
          />
        </div>
        <form.Field
          name="middleName"
          children={(field) => (
            <label className="flex w-13rem text-xs flex-column my-2">
              Отчество*
              <InputText
                className="mt-1"
                id={field.name}
                disabled={isSubmitted}
                name={field.name}
                value={middleName ? middleName : (field.state.value ?? '')}
                onBlur={field.handleBlur}
                onChange={(e) => field.handleChange(e.target.value)}
                placeholder="Введите"
              />
            </label>
          )}
        />
        <span className="text-xs mb-3">При изменении имени и фамилии в аккаунте, они также изменятся в профиле</span>
        {currentActivityCode === 'individualEntrepreneur' && (
          <form.Field
            name="address"
            children={(field) => (
              <label className="mt-2 text-xs" style={{ marginRight: '224px' }}>
                Фактический адрес*
                <InputTextarea
                  id={field.name}
                  disabled={isSubmitted}
                  name={field.name}
                  value={accountData?.address ?? field.state.value ?? ''}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                  rows={3}
                  className="mt-2 w-full"
                  placeholder="Введите"
                />
              </label>
            )}
          />
        )}
        {/* <div className="flex gap-3 mb-4 mt-3">
            <form.Field
              name="checkedRules"
              children={(field) => (
                <Checkbox
                  inputId="rules"
                  id={field.name}
                  name={field.name}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(!!e.target.checked)}
                  checked={Boolean(field.state.value)}
                  className="ml-1"
                />
              )}
            />
            <label htmlFor="rules" className="text-primary text-xs">
              Соглашаюсь с правилами взаимодействия на площадке
            </label>
          </div> */}
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
                loading={pending}
                className="mb-4 w-13rem"
                onClick={() => setModalWindow(false)}
              />
              <Button
                type="button"
                onClick={() => {
                  setModalWindow(false);
                  if (isNew === false) {
                    updateAccountEvent({ formType: 'private' });
                  } else {
                    updateIsSubmitted(true);
                    accountOptionsQuery.start();
                  }
                }}
                label="Отправить"
                loading={pending}
                className="mb-4 w-13rem"
              />
            </div>
          </div>
        </Dialog>
        <form.Subscribe
          selector={(state) => ({
            canSubmit: state.isTouched && state.canSubmit,
            address: state.values.address,
            type: state.values.type,
          })}
        >
          {({ canSubmit, address, type }) => {
            const requireType = type !== '';
            const requireAddress = type === 'individualEntrepreneur';
            const isAddressValid = !requireAddress || (address && address.trim() !== '');
            return canSubmit && isAddressValid && requireType ? (
              <Button
                type="button"
                disabled={isSubmitted}
                label={isNew === false ? 'Сохранить' : 'Продолжить'}
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
