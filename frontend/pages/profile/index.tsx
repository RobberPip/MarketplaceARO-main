import { useUnit } from 'effector-react';
import { Button } from 'primereact/button';
import { Divider } from 'primereact/divider';
import { InputSwitch } from 'primereact/inputswitch';

import { Dialog } from 'primereact/dialog';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { Message } from 'primereact/message';
import { useState } from 'react';
import { Dropdown } from '~/shared/ui/dropdown';
import {
  $companySuggestValue,
  $companyValue,
  $email,
  $industries,
  $isEmailSend,
  $isNextStep,
  $login,
  $roles,
  $timeLeft,
  changeEmailForm,
  changePasswordForm,
  profileForm,
  updateIsEmailSend,
  updateIsNextStep,
  updateTimeLeft,
  updateCompanyValue,
  updateProfileMutation,
  updateEmailMutation,
  updatePasswordMutation,
  updateLoginMutation,
  changeLoginForm,
  closeModal,
  openPassModal,
  $isPassModalOpen,
  profileQuery,
  avatarUploadMutation,
} from './model.ts';
import { CustomAvatar } from '../../shared/ui/components/avatar.tsx';
import { Password } from 'primereact/password';
import { useInterval } from 'primereact/hooks';
import { PasswordError } from '~/shared/ui/components/index.ts';
import { formatTime } from '~/shared/lib/format.ts';

export const ProfileView = () => {
  const { pending: pendingProfile } = useUnit(updateProfileMutation);
  const { pending: pendingEmail } = useUnit(updateEmailMutation);
  const { pending: pendingLogin } = useUnit(updateLoginMutation);
  const { pending: pendingPassword } = useUnit(updatePasswordMutation);
  const [industries, roles, email, login] = useUnit([$industries, $roles, $email, $login]);
  const { extendedForm: form } = useUnit(profileForm);
  const { extendedForm: formEmail } = useUnit(changeEmailForm);
  const { extendedForm: formLogin } = useUnit(changeLoginForm);
  const { extendedForm: formPass } = useUnit(changePasswordForm);
  const [loginModal, setLoginModal] = useState<boolean>(false);
  const [emailModal, setEmailModal] = useState<boolean>(false);

  const [isPassModalOpen, openPassModalEvent, closeModalEvent] = useUnit([$isPassModalOpen, openPassModal, closeModal]);
  const data = useUnit(profileQuery.$data);
  // Timers send email
  const [timeLeft, updateTimeLeftCall] = useUnit([$timeLeft, updateTimeLeft]);
  const [isEmailSend, updateIsEmailSendCall] = useUnit([$isEmailSend, updateIsEmailSend]);
  const [isNextStep, updateIsNextStepCall] = useUnit([$isNextStep, updateIsNextStep]);
  const avatarUrl = data.avatar?.url ? `${data.avatar.url}?t=${data.avatar.updatedAt}` : '';
  useInterval(() => {
    if (timeLeft > 0) {
      updateTimeLeftCall(timeLeft - 1);
    } else {
      updateIsEmailSendCall(false);
    }
  }, 1000);

  return (
    <>
      <main className="mb-4">
        <article className="flex flex-column gap-4 mb-4">
          <h1 className="m-0 text-2xl font-medium">Профиль</h1>
          <section>
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
              <h2 className="m-0 text-lg font-medium">Основная информация</h2>
              <Divider className="mt-2 mb-3" />
              <div className="flex justify-content-between">
                <div className="flex flex-column w-full text-color-secondary gap-2">
                  <div className="flex flex-row gap-5 w-full">
                    <form.Field
                      name="firstName"
                      children={(field) => (
                        <label className="flex w-13rem text-xs flex-column">
                          Имя
                          <InputText
                            className=" mt-1"
                            placeholder="Введите"
                            id={field.name}
                            name={field.name}
                            value={field.state.value}
                            onBlur={field.handleBlur}
                            onChange={(e) => field.handleChange(e.target.value)}
                            invalid={field.state.meta.errors.length > 0}
                          />
                          {field.state.meta.errors.length > 0 && (
                            <Message
                              className="w-full mt-1"
                              severity="error"
                              text={field.state.meta.errors.join('\n')}
                            />
                          )}
                        </label>
                      )}
                    />
                    <form.Field
                      name="lastName"
                      children={(field) => (
                        <label className="flex w-13rem text-xs flex-column">
                          Фамилия
                          <InputText
                            className="mt-1"
                            placeholder="Введите"
                            id={field.name}
                            name={field.name}
                            value={field.state.value}
                            onBlur={field.handleBlur}
                            onChange={(e) => field.handleChange(e.target.value)}
                            invalid={field.state.meta.errors.length > 0}
                          />
                          {field.state.meta.errors.length > 0 && (
                            <Message
                              className="w-full mt-1"
                              severity="error"
                              text={field.state.meta.errors.join('\n')}
                            />
                          )}
                        </label>
                      )}
                    />
                  </div>
                  <span className="text-xs">
                    При активации приватного аккаунта поля перестанут быть доступными для редактирования. Укажите верные
                    данные заранее
                  </span>
                </div>
                <CustomAvatar className="ml-8" avatarUrl={avatarUrl} updateMutation={avatarUploadMutation} />
              </div>

              <div className="flex flex-column gap-2 mt-3 mb-4" style={{ marginRight: '224px' }}>
                <form.Field
                  name="description"
                  children={(field) => (
                    <label className="flex flex-column text-xs text-color-secondary gap-2">
                      Описание
                      <InputTextarea
                        rows={3}
                        className="w-full"
                        placeholder="Введите"
                        id={field.name}
                        name={field.name}
                        value={field.state.value ?? ''}
                        onBlur={field.handleBlur}
                        onChange={(e) => field.handleChange(e.target.value)}
                      />
                    </label>
                  )}
                />
              </div>
              <h2 className="m-0 text-lg font-medium">Дополнительная информация</h2>
              <Divider className="mt-2 mb-3" />

              <div className="flex flex-column lg:w-29rem w-full text-color-secondary gap-3">
                <label className="flex text-xs flex-column gap-1">
                  Компания
                  <SelectCompany />
                  <span className="text-xs">
                    Укажите компанию в которой вы работаете. Вы сможете использовать компанию при создании аккаунта.
                  </span>
                </label>
                <form.Field
                  name="industryId"
                  children={(field) => (
                    <label className="flex text-xs flex-column gap-1">
                      Индустрия
                      <Dropdown
                        placeholder="Введите или выберите"
                        id={field.name}
                        value={field.state.value}
                        optionLabel={'name'}
                        optionValue={'id'}
                        filterInputAutoFocus
                        resetFilterOnHide
                        onChange={(e) => field.handleChange(e.target.value)}
                        options={industries}
                      />
                      <span className="mt-1 text-xs">
                        Укажите вашу индустрию, чтобы мы смогли подобрать для вас наиболее подходящие цифровые продукты
                      </span>
                    </label>
                  )}
                />

                <form.Field
                  name="roleId"
                  children={(field) => {
                    return (
                      <label className="flex text-xs flex-column gap-1 mb-4">
                        Роль
                        <Dropdown
                          placeholder="Введите или выберите"
                          id={field.name}
                          value={field.state.value}
                          optionLabel="name"
                          optionValue="id"
                          optionGroupLabel="label"
                          optionGroupChildren="items"
                          filterInputAutoFocus
                          resetFilterOnHide
                          onChange={(e) => field.handleChange(e.value)}
                          options={roles}
                        />
                        <span className="mt-1 text-xs">
                          Укажите вашу роль, чтобы мы смогли предлагать вам наиболее актуальные цифровые продукты для
                          использования
                        </span>
                      </label>
                    );
                  }}
                />
              </div>

              <form.Subscribe
                selector={(state) => [state.canSubmit, state.isSubmitting, state.isDirty, state.isFieldsValid]}
                children={([canSubmit, isSubmitting, isDirty, isFieldsValid]) => {
                  return (
                    <Button
                      label="Сохранить"
                      loading={pendingProfile}
                      disabled={!canSubmit || !isDirty || !isFieldsValid}
                      className="w-7rem"
                      type="submit"
                    />
                  );
                }}
              />
            </form>
          </section>
          <section>
            <h2 className="m-0 text-lg font-medium">Настройки безопасности</h2>
            <Divider className="mt-2 mb-3" />
            <div className="flex flex-column gap-3">
              <div className="flex justify-content-between">
                <div className="flex flex-column">
                  <h3 className="m-0 font-semibold text-base">Логин</h3>
                  <span className="text-sm mt-2">{login}</span>
                  <span className="text-xs text-color-secondary mt-1">
                    Установите логин для вашего профиля, чтобы получить доступ к цифровым продуктам платформы IT market
                    с помощью ARO коннектора
                  </span>
                </div>
                <div className="flex align-items-center">
                  <Button
                    className="w-9rem"
                    severity="secondary"
                    size="small"
                    label="Изменить логин"
                    onClick={() => setLoginModal(true)}
                  />
                  <Dialog
                    header="Изменение логина"
                    headerClassName="pb-3"
                    className="text-sm"
                    visible={loginModal}
                    style={{ width: '512px' }}
                    onHide={() => {
                      if (!loginModal) return;
                      setLoginModal(false);
                      formLogin.reset();
                      updateIsNextStepCall(false);
                    }}
                  >
                    <form
                      onSubmit={(e) => {
                        e.preventDefault();
                        formLogin.handleSubmit();
                      }}
                    >
                      {isNextStep ? (
                        <div>
                          Для завершения изменения мы отправили ссылку подтверждения на вашу новую электронную почту:
                          <p>
                            <formEmail.Field name="email" children={(field) => <strong>{field.state.value}</strong>} />
                          </p>
                          <p className="mb-4">Убедитесь, что вам пришло письмо</p>
                          {timeLeft > 0 && (
                            <>
                              <div className="mb-2">Отправить повторно через</div>
                              <div className="mb-4 font-semibold text-base">{formatTime(timeLeft)}</div>
                            </>
                          )}
                          <formEmail.Subscribe
                            selector={(state) => [state.canSubmit, state.isSubmitting, state.isDirty]}
                            children={() => {
                              return (
                                <>
                                  <Button
                                    label="Повторно отправить ссылку"
                                    loading={pendingEmail}
                                    severity="secondary"
                                    className="w-14rem mb-4"
                                    disabled={isEmailSend}
                                    type="submit"
                                  />
                                </>
                              );
                            }}
                          />
                        </div>
                      ) : (
                        <div>
                          Введите новый логин для вашего профиля
                          <formLogin.Field
                            name="login"
                            children={(field) => (
                              <label className="flex mt-3 gap-1 w-21rem text-xs flex-column text-color-secondary">
                                Новый логин
                                <InputText
                                  className=" mt-1"
                                  placeholder="Введите"
                                  id={field.name}
                                  name={field.name}
                                  value={field.state.value}
                                  onBlur={field.handleBlur}
                                  onChange={(e) => field.handleChange(e.target.value)}
                                  invalid={field.state.meta.errors.length > 0}
                                />
                                {field.state.meta.errors.some(
                                  (error) => error && typeof error === 'string' && error.includes('server'),
                                ) ? (
                                  <Message
                                    className="w-full mt-1"
                                    severity="error"
                                    text={'Введенный логин уже занят'}
                                  />
                                ) : (
                                  field.state.meta.errors.length > 0 && (
                                    <Message
                                      className="w-full mt-1"
                                      severity="error"
                                      text={field.state.meta.errors.filter(Boolean).join('/n')}
                                    />
                                  )
                                )}
                              </label>
                            )}
                          />
                          <Divider className="my-4" />
                          <div className="flex justify-content-between mb-4">
                            <formLogin.Subscribe
                              selector={(state) => [state.canSubmit, state.isDirty, state.isValid, state.isSubmitting]}
                              children={([canSubmit, isDirty, isValid, isSubmitting]) => (
                                <>
                                  <Button
                                    label="Отменить"
                                    severity="secondary"
                                    type="reset"
                                    onClick={() => {
                                      setLoginModal(false);
                                      formLogin.reset();
                                      updateIsNextStepCall(false);
                                    }}
                                    className="w-13rem"
                                  />
                                  <Button
                                    label="Изменить логин"
                                    loading={pendingLogin}
                                    disabled={!canSubmit || !isDirty || !isValid || isSubmitting}
                                    className="w-13rem"
                                    type="submit"
                                  />
                                </>
                              )}
                            />
                          </div>
                        </div>
                      )}
                    </form>
                  </Dialog>
                </div>
              </div>
              <div className="flex justify-content-between">
                <div className="flex flex-column">
                  <h3 className="m-0 font-semibold text-base">Email</h3>
                  <span className="text-sm mt-2">{email}</span>
                </div>
                <div className="flex align-items-center">
                  <Button
                    className="w-9rem"
                    severity="secondary"
                    label="Изменить почту"
                    onClick={() => setEmailModal(true)}
                  />
                  <Dialog
                    header="Изменение почты"
                    className="text-sm"
                    headerClassName="pb-3"
                    visible={emailModal}
                    style={{ width: '512px' }}
                    onHide={() => {
                      if (!emailModal) return;
                      setEmailModal(false);
                      formEmail.reset();
                      updateIsNextStepCall(false);
                    }}
                  >
                    <form
                      onSubmit={(e) => {
                        e.preventDefault();
                        formEmail.handleSubmit();
                      }}
                    >
                      {isNextStep ? (
                        <div>
                          Для завершения изменения мы отправили ссылку подтверждения на вашу новую электронную почту:
                          <p>
                            <formEmail.Field name="email" children={(field) => <strong>{field.state.value}</strong>} />
                          </p>
                          <p className="mb-4">Убедитесь, что вам пришло письмо</p>
                          {timeLeft > 0 && (
                            <>
                              <div className="mb-2">Отправить повторно через</div>
                              <div className="mb-4 font-semibold text-base">{formatTime(timeLeft)}</div>
                            </>
                          )}
                          <formEmail.Subscribe
                            selector={(state) => [state.canSubmit, state.isSubmitting, state.isDirty]}
                            children={() => {
                              return (
                                <>
                                  <Button
                                    label="Повторно отправить ссылку"
                                    loading={pendingEmail}
                                    severity="secondary"
                                    className="w-14rem mb-4"
                                    disabled={isEmailSend}
                                    type="submit"
                                  />
                                </>
                              );
                            }}
                          />
                        </div>
                      ) : (
                        <div>
                          Введите новую электронную почту, и мы отправим на неё ссылку для подтверждения
                          <formEmail.Field
                            name="email"
                            children={(field) => (
                              <label className="flex mt-3 gap-1 w-21rem text-xs flex-column text-color-secondary">
                                Новая почта
                                <InputText
                                  className=" mt-1"
                                  placeholder="Введите"
                                  id={field.name}
                                  name={field.name}
                                  value={field.state.value}
                                  onBlur={field.handleBlur}
                                  onChange={(e) => field.handleChange(e.target.value)}
                                  invalid={field.state.meta.errors.length > 0}
                                />
                                {field.state.meta.errors.some(
                                  (error) => error && typeof error === 'string' && error.includes('server'),
                                ) ? (
                                  <Message
                                    className="w-full mt-1"
                                    severity="error"
                                    text={'Введенная почта уже занята'}
                                  />
                                ) : (
                                  field.state.meta.errors.length > 0 && (
                                    <Message
                                      className="w-full mt-1"
                                      severity="error"
                                      text={field.state.meta.errors.filter(Boolean).join('/n')}
                                    />
                                  )
                                )}
                              </label>
                            )}
                          />
                          <Divider className="my-4" />
                          <div className="flex justify-content-between mb-4">
                            <formEmail.Subscribe
                              selector={(state) => [state.canSubmit, state.isDirty, state.isValid, state.isSubmitting]}
                              children={([canSubmit, isDirty, isValid, isSubmitting]) => (
                                <>
                                  <Button
                                    label="Отменить"
                                    severity="secondary"
                                    type="reset"
                                    onClick={() => {
                                      setEmailModal(false);
                                      formEmail.reset();
                                      updateIsNextStepCall(false);
                                    }}
                                    className="w-13rem"
                                  />
                                  <Button
                                    label="Изменить почту"
                                    loading={pendingEmail}
                                    disabled={!canSubmit || !isDirty || !isValid || isSubmitting}
                                    className="w-13rem"
                                    type="submit"
                                  />
                                </>
                              )}
                            />
                          </div>
                        </div>
                      )}
                    </form>
                  </Dialog>
                </div>
              </div>

              <div className="flex justify-content-between">
                <div className="flex flex-column">
                  <h3 className="m-0 font-semibold text-base">Пароль</h3>
                  <span className="text-color-secondary text-xs mt-2">
                    Установите постоянный пароль для входа в свою учётную запись
                  </span>
                </div>
                <div className="flex align-items-center">
                  <Button
                    className="w-10rem"
                    label="Изменить пароль"
                    severity="secondary"
                    onClick={() => openPassModalEvent()}
                  />
                  <Dialog
                    header="Изменение пароля"
                    headerClassName="pb-3"
                    className="text-sm"
                    visible={isPassModalOpen}
                    style={{ width: '512px' }}
                    onHide={() => {
                      if (!isPassModalOpen) return;
                      closeModalEvent();
                      formPass.reset();
                    }}
                  >
                    <form
                      id="myForm"
                      onSubmit={(e) => {
                        e.preventDefault();
                        formPass.handleSubmit();
                      }}
                    >
                      <div>
                        Для изменения пароля введите сперва старый пароль, а затем придумайте новый
                        <formPass.Field
                          name="oldPassword"
                          children={(field) => (
                            <label className="flex mt-3 mb-2 gap-1 w-21rem text-xs flex-column text-color-secondary">
                              Старый пароль
                              <Password
                                id={field.name}
                                name={field.name}
                                value={field.state.value}
                                onBlur={field.handleBlur}
                                onChange={(e) => field.handleChange(e.target.value)}
                                invalid={field.state.meta.errors.length > 0}
                                placeholder="Введите"
                                toggleMask
                                feedback={false}
                              />
                              {field.state.meta.errors.length > 0 && (
                                <Message className="w-full mt-1" severity="error" text={'Вы ввели не верный пароль'} />
                              )}
                            </label>
                          )}
                        />
                        <formPass.Field
                          name="newPassword"
                          children={(field) => (
                            <label className="flex gap-1 w-21rem text-xs flex-column text-color-secondary">
                              Новый пароль
                              <Password
                                id={field.name}
                                name={field.name}
                                value={field.state.value}
                                onBlur={field.handleBlur}
                                onChange={(e) => field.handleChange(e.target.value)}
                                invalid={field.state.meta.errors.filter((error) => error !== '').length > 0}
                                placeholder="Введите"
                                toggleMask
                                feedback={false}
                              />
                              {field.state.meta.errors.length > 0 && (
                                <PasswordError passwordError={field.state.meta.errors.join('/n')} />
                              )}
                            </label>
                          )}
                        />
                        <Divider className="my-4" />
                        <div className="flex justify-content-between mb-4">
                          <formPass.Subscribe
                            selector={(state) => [state.canSubmit, state.isDirty, state.isValid, state.isSubmitting]}
                            children={([canSubmit, isDirty, isValid, isSubmitting]) => (
                              <>
                                <Button
                                  label="Отменить"
                                  type="reset"
                                  onClick={() => {
                                    closeModalEvent();
                                    formPass.reset();
                                  }}
                                  severity="secondary"
                                  className="w-13rem"
                                />
                                <Button
                                  label="Изменить пароль"
                                  disabled={!canSubmit || !isDirty || !isValid || isSubmitting}
                                  loading={pendingPassword}
                                  className="w-13rem"
                                  type="submit"
                                />
                              </>
                            )}
                          />
                        </div>
                      </div>
                    </form>
                  </Dialog>
                </div>
              </div>

              {/* <div className="flex justify-content-between">
                <div className="flex flex-column">
                  <h3 className="m-0 font-semibold text-base">Подтверждение входа</h3>
                  <span className="text-color-secondary text-xs mt-2">
                    Добавьте дополнительный уровень безопасности для вашего профиля во время входа в систему
                  </span>
                </div>
                <InputSwitch
                  className="mt-2"
                  disabled={true}
                  checked={passwordChecked}
                  onChange={(e) => setPasswordChecked(e.value)}
                />
              </div> */}
            </div>
          </section>
          <section className="flex flex-column">
            <h2 className="m-0 text-lg font-medium">Удаление профиля</h2>
            <Divider className="mt-2 mb-3" />

            <span className="text-xs text-color-secondary mb-4">
              Удаление профиля приведёт к потере доступа к платформе IT market. Ваши личные данные также будут удалены
              <br />
              без возможности восстановления. Для удаления профиля обратитесь в поддержку на почту update@arogroup.ru
            </span>

            {/* <DeleteProfile /> */}
          </section>
        </article>
      </main>
    </>
  );
};

function SelectCompany() {
  const model = useUnit({ value: $companyValue, update: updateCompanyValue });
  const suggest = useUnit($companySuggestValue);
  const updateCompanyCall = useUnit(updateCompanyValue);

  return (
    <div className="relative" style={{ height: '37px' }}>
      <InputText className="absolute text-400 pointer-events-none w-full" value={suggest} />
      <InputText
        placeholder="Введите"
        className="absolute w-full"
        style={{ backgroundColor: 'transparent' }}
        onChange={(e) => model.update({ companyName: e.target.value })}
        onKeyDown={(k) => {
          if (k.code === 'ArrowRight' || k.code === 'Enter') {
            updateCompanyValue({ companyName: suggest });
          }
        }}
        value={model.value}
      />
    </div>
  );
}
