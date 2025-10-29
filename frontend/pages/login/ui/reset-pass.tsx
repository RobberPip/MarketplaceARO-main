import { useUnit } from 'effector-react';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { Message } from 'primereact/message';
import { $passwordError, forgotPassForm, resetPassForm } from '../model';
import { PasswordError } from '~/shared/ui/components';
import { useState } from 'react';
import { useInterval } from 'primereact/hooks';
import { formatTime } from '~/shared/lib/format';
import { Password } from 'primereact/password';

export const ForgotPass = () => {
  const { extendedForm: form } = useUnit(forgotPassForm);
  const [timeLeft, setTimeLeft] = useState(0);
  const [buttonDisabled, setButtonDisabled] = useState(true);
  // TODO Заменить на (https://share.effector.dev/maL70QHS)
  useInterval(() => {
    if (timeLeft > 0) {
      setTimeLeft(timeLeft - 1);
    } else {
      setButtonDisabled(false);
    }
  }, 1000);

  return (
    <div className="flex flex-column align-items-center my-8">
      <h1 className="text-center font-medium text-2xl mt-7 mb-4 max-w-29rem">
        Восстановление
        <br />
        пароля
      </h1>
      <form
        onSubmit={(e) => {
          e.preventDefault();
          form.handleSubmit();
          setTimeLeft(60);
          setButtonDisabled(true);
        }}
        onKeyDown={(e) => {
          if (e.code === 'Enter') {
            e.preventDefault();
          }
        }}
      >
        <div className="flex flex-column w-full sm:w-21rem justify-content-center gap-2">
          <div className="text-color-secondary flex flex-column gap-3">
            <div className="flex flex-column gap-1">
              <form.Field
                name="email"
                children={(field) => (
                  <label htmlFor="login">
                    Почта*
                    <InputText
                      id={field.name}
                      name={field.name}
                      value={field.state.value}
                      onBlur={field.handleBlur}
                      onChange={(e) => field.handleChange(e.target.value)}
                      invalid={field.state.meta.errors.length > 0}
                      placeholder="Введите"
                      autoComplete="email"
                      className="w-full"
                    />
                    {field.state.meta.errors.length > 0 && (
                      <Message className="w-full mt-1" severity="error" text={field.state.meta.errors.join('\n')} />
                    )}
                  </label>
                )}
              />
            </div>
          </div>
          <form.Subscribe
            selector={(state) => [state.canSubmit, state.isDirty, state.isFieldsValid]}
            children={([canSubmit, isDirty, isFieldsValid]) => {
              return (
                <>
                  <Button
                    label="Отправить ссылку"
                    type="submit"
                    disabled={!canSubmit || !isDirty || !isFieldsValid || buttonDisabled}
                    className="sm:w-21rem w-full mt-3 mb-4"
                  />
                  {timeLeft > 0 && (
                    <>
                      <div className="mb-2 text-center">Отправить повторно через</div>
                      <div className="mb-4 text-center font-semibold text-base">{formatTime(timeLeft)}</div>
                    </>
                  )}
                  <div className="text-sm flex flex-column text-center">
                    <span>Возникли проблемы с восстановлением?</span>
                    <span>
                      Напишите нам нам почту <span className="underline">support@arogroup.ru</span>
                    </span>
                  </div>
                </>
              );
            }}
          />
        </div>
      </form>
    </div>
  );
};

export const ResetPass = () => {
  const passwordError = useUnit($passwordError);
  const { extendedForm: form } = useUnit(resetPassForm);
  return (
    <div className="flex flex-column align-items-center my-8">
      <h1 className="text-center font-medium text-2xl mt-7 mb-4 max-w-29rem">
        Восстановление
        <br />
        пароля
      </h1>
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
        <div className="flex flex-column w-full sm:w-21rem justify-content-center gap-2">
          <div className="text-color-secondary flex flex-column gap-3">
            <div className="flex flex-column gap-1">
              <form.Field
                name="newPassword"
                children={(field) => (
                  <label htmlFor="login">
                    Новый пароль*
                    <Password
                      feedback={false}
                      toggleMask
                      id={field.name}
                      name={field.name}
                      value={field.state.value}
                      onBlur={field.handleBlur}
                      onChange={(e) => field.handleChange(e.target.value)}
                      invalid={field.state.meta.errors.length > 0}
                      placeholder="Введите"
                      autoComplete="password"
                      className="w-full"
                    />
                    {field.state.meta.errors.length > 0 && (
                      <PasswordError passwordError={field.state.meta.errors.join('/n')} />
                    )}
                  </label>
                )}
              />
            </div>
          </div>
          <form.Subscribe
            selector={(state) => [state.canSubmit, state.isDirty, state.isFieldsValid]}
            children={([canSubmit, isDirty, isFieldsValid]) => {
              return (
                <Button
                  label="Войти"
                  type="submit"
                  disabled={!canSubmit || !isDirty || !isFieldsValid}
                  className="sm:w-21rem w-full mt-3 mb-2"
                />
              );
            }}
          />
        </div>
      </form>
    </div>
  );
};
