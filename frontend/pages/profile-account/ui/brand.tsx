import { Button } from 'primereact/button';
import { Divider } from 'primereact/divider';
import { Dropdown } from 'primereact/dropdown';
import { InputSwitch } from 'primereact/inputswitch';
import { InputMask } from 'primereact/inputmask';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { useState } from 'react';
import { CustomAvatar } from '~/shared/ui/components';
import {
  $hasAnyValueBrand,
  $mainActivity,
  $previewUrl,
  $publicLists,
  $selectedFile,
  accountQuery,
  addPrivateAccountForm,
  addPublicAccountForm,
  avatarUploadMutation,
  setSelectedFile,
  updateBrandForm,
  updateBrandMutation,
} from '../model';
import { useUnit } from 'effector-react';
import { MultiSelect } from 'primereact/multiselect';
import { Message } from 'primereact/message';
import { StatusIcon } from '..';

export function BrandAccount({
  publicAccount,
  disabled,
  new: isNew,
}: {
  publicAccount: boolean;
  disabled?: boolean;
  new?: boolean;
}) {
  const accountData = isNew ? null : useUnit(accountQuery.$data);
  const avatarUrl = accountData?.brand?.avatar?.url
    ? `${accountData?.brand.avatar.url}?t=${accountData?.brand.avatar.updatedAt}`
    : '';
  const { extendedForm: form } = useUnit(updateBrandForm);
  const { pending } = useUnit(updateBrandMutation);
  const [publicLists] = useUnit([$publicLists]);
  const [mainActivity] = useUnit([$mainActivity]);
  const isDisabled = isNew
    ? !!disabled
    : accountData?.userRole !== 'Owner' || !!disabled || accountData?.brand?.status === 'Pending';
  const { extendedForm: accountForm } = useUnit(publicAccount ? addPublicAccountForm : addPrivateAccountForm);
  const previewUrl = useUnit($previewUrl);
  const [avatarTouched, setAvatarTouched] = useState(false);
  return (
    <>
      <h3 className="m-0 text-2xl font-semibold">
        {accountData?.brand?.publicName ?? 'Бренд'}
        <StatusIcon status={accountData?.brand?.status} />
      </h3>

      <section className="flex flex-column text-color-secondary">
        {disabled && (
          <span className="text-xs mt-4">
            Сохраните информацию об аккаунте, чтобы добавить маркетинговые данные о вашем бренде
          </span>
        )}

        {!disabled && (
          <div>
            <h2 className="m-0 text-lg mt-4 font-medium text-primary">Маркетинговые данные</h2>
            <Divider className="mt-2 mb-3" />
            <form
              onSubmit={(e) => {
                e.preventDefault();
                form.handleSubmit();
              }}
            >
              <div className="flex justify-content-between">
                <div className="w-full">
                  <form.Field
                    name="publicName"
                    children={(field) => (
                      <label className="flex w-13rem text-xs flex-column">
                        Публичное имя*
                        <InputText
                          className="w-28rem mt-1"
                          id={field.name}
                          name={field.name}
                          disabled={isDisabled}
                          invalid={field.state.meta.errors.length > 0}
                          value={field.state.value ?? ''}
                          onBlur={field.handleBlur}
                          onChange={(e) => field.handleChange(e.target.value)}
                          placeholder="Введите"
                        />
                      </label>
                    )}
                  />
                  <form.Field
                    name="description"
                    children={(field) => (
                      <label className=" w-full flex flex-column text-xs text-color-secondary gap-2 my-2">
                        Описание
                        <InputTextarea
                          rows={3}
                          id={field.name}
                          name={field.name}
                          disabled={isDisabled}
                          value={field.state.value ?? ''}
                          onBlur={field.handleBlur}
                          onChange={(e) => field.handleChange(e.target.value)}
                          className="w-full"
                          placeholder="Введите краткое описание"
                        />
                      </label>
                    )}
                  />
                  <label className="flex text-xs flex-column">
                    <form.Field
                      name="mainActivityIds"
                      children={(field) => (
                        <label className="flex text-xs flex-column">
                          Основная деятльность
                          <MultiSelect
                            className="w-28rem mt-1"
                            id={field.name}
                            disabled={isDisabled}
                            value={field.state.value ?? ''}
                            optionLabel={'name'}
                            optionValue={'id'}
                            filterInputAutoFocus
                            resetFilterOnHide
                            onChange={(e) => field.handleChange(e.target.value)}
                            options={mainActivity}
                            placeholder="Введите"
                          />
                        </label>
                      )}
                    />
                    <span className="mt-2 text-xs">
                      При активации приватного аккаунта поля перестанут быть доступными для
                      <br /> редактирования. Укажите верные данные заранее
                    </span>
                  </label>
                  {publicAccount && (
                    <div>
                      <h2 className="m-0 text-lg mt-3 font-medium text-primary">Публичные данные</h2>
                      <Divider className="mt-2 mb-3" />
                      <form.Field
                        name="publicListIds"
                        children={(field) => (
                          <label className="flex text-xs flex-column">
                            Публичные списки
                            <MultiSelect
                              className="w-28rem mt-1"
                              id={field.name}
                              disabled={isDisabled}
                              value={field.state.value ?? ''}
                              optionLabel={'name'}
                              optionValue={'id'}
                              filterInputAutoFocus
                              resetFilterOnHide
                              onChange={(e) => field.handleChange(e.target.value)}
                              options={publicLists}
                              placeholder="Введите"
                            />
                          </label>
                        )}
                      />
                    </div>
                  )}
                  <h2 className="m-0 text-lg mt-4 font-medium text-primary">Контакты</h2>
                  <Divider className="mt-2 mb-3" />
                  <div className="flex w-28rem flex-column gap-2">
                    <form.Field
                      name="website"
                      children={(field) => (
                        <label className="flex text-xs flex-column">
                          Сайт
                          <InputText
                            className="mt-1"
                            id={field.name}
                            name={field.name}
                            disabled={isDisabled}
                            value={field.state.value ?? ''}
                            onBlur={field.handleBlur}
                            onChange={(e) => field.handleChange(e.target.value)}
                            placeholder="https://"
                          />
                        </label>
                      )}
                    />
                    <form.Field
                      name="email"
                      children={(field) => (
                        <label className="flex text-xs flex-column">
                          Почта
                          <InputText
                            className="mt-1"
                            id={field.name}
                            name={field.name}
                            disabled={isDisabled}
                            value={field.state.value ?? ''}
                            onBlur={field.handleBlur}
                            onChange={(e) => field.handleChange(e.target.value)}
                            placeholder="Введите"
                          />
                        </label>
                      )}
                    />
                    <form.Field
                      name="phoneNumber"
                      children={(field) => (
                        <label className="flex text-xs flex-column">
                          Телефон
                          <InputMask
                            className="mt-1"
                            id={field.name}
                            name={field.name}
                            disabled={isDisabled}
                            value={field.state.value ?? ''}
                            onBlur={field.handleBlur}
                            onChange={(e) => field.handleChange(e.target.value)}
                            mask="+7 (999) 999-99-99"
                            placeholder="+7 (__) ___-__-__"
                          />
                        </label>
                      )}
                    />
                  </div>
                  <form.Subscribe
                    selector={(state) => (state.isTouched || avatarTouched) && state.canSubmit}
                    children={(canSubmit) => {
                      if (isNew) {
                        return (
                          <Button
                            label="Сохранить"
                            type="button"
                            onClick={(e) => {
                              e.preventDefault();
                              accountForm.handleSubmit();
                            }}
                            visible={canSubmit && !isDisabled}
                            loading={pending}
                            className="mt-4"
                          />
                        );
                      }
                      return (
                        <Button label="Сохранить" type="submit" visible={canSubmit && !isDisabled} className="mt-4" />
                      );
                    }}
                  />
                </div>
                <div className="ml-7">
                  <CustomAvatar
                    className="ml-8"
                    disabled={isDisabled}
                    avatarUrl={avatarUrl}
                    previewUrl={previewUrl ?? undefined}
                    onFileSelect={(file) => {
                      setSelectedFile(file);
                      form.validateAllFields('submit');
                      setAvatarTouched(true);
                    }}
                    updateMutation={avatarUploadMutation}
                    autoUpload={false}
                  />
                </div>
              </div>
            </form>
          </div>
        )}
      </section>
    </>
  );
}
