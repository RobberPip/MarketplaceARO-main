import { useUnit } from 'effector-react';
import { Button } from 'primereact/button';
import { Checkbox } from 'primereact/checkbox';
import { InputText } from 'primereact/inputtext';
import { Message } from 'primereact/message';
import { Password } from 'primereact/password';
import { routes } from '~/shared/routing.ts';
import { $formError, loginForm, registrationForm } from '../model';
import { Link } from 'atomic-router-react';
import { signInMutation } from '~/shared/auth';

export const SignIn = () => {
  const [formError] = useUnit([$formError]);
  const { pending: pendingLogin } = useUnit(signInMutation);
  const { extendedForm: form } = useUnit(loginForm);
  return (
    <div className="flex flex-column align-items-center my-8">
      <h1 className="mt-7 mb-4 max-w-29rem font-meduim text-2xl">Вход</h1>
      <div className="flex flex-column w-full sm:w-21rem justify-content-center gap-2">
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
                      placeholder="Введите"
                      invalid={!!formError || (field.state.meta.errors?.length ?? 0) > 0}
                      className="w-full"
                      autoComplete="username"
                    />
                    {field.state.meta.errors?.length > 0 && (
                      <Message className="w-full mt-1" severity="error" text={field.state.meta.errors.join('\n')} />
                    )}
                  </label>
                )}
              />
            </div>
            <div className="flex flex-column gap-1">
              <div className="flex justify-content-between">
                <label htmlFor="password">Пароль*</label>
                <Link to={routes.resetpass} className="no-underline text-indigo-500">
                  Забыли пароль?
                </Link>
              </div>
              <form.Field
                name="password"
                children={(field) => (
                  <>
                    <Password
                      id={field.name}
                      name={field.name}
                      value={field.state.value}
                      onBlur={field.handleBlur}
                      onChange={(e) => field.handleChange(e.target.value)}
                      feedback={false}
                      invalid={!!formError || field.state.meta.errors.length > 0}
                      toggleMask
                      autoComplete="current-password"
                      placeholder="Введите"
                    />
                    {!!formError && <Message severity="error" text={<span>Логин/почта или пароль не подходят</span>} />}
                  </>
                )}
              />
            </div>
          </div>
          <div className="flex gap-3 mt-2 justify-center w-full sm:w-21rem">
            <form.Field
              name="rememberMe"
              children={(field) => (
                <Checkbox
                  inputId="remember"
                  className="ml-1"
                  id={field.name}
                  name={field.name}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(!!e.target.checked)}
                  checked={Boolean(field.state.value)}
                />
              )}
            />

            <label htmlFor="remember">Запомнить меня</label>
          </div>
          <form.Subscribe
            selector={(state) => [state.canSubmit, state.isDirty, state.isFieldsValid, state.values]}
            children={([canSubmit, isDirty, isFieldsValid, values]) => {
              const isAllFieldsFilled = Object.values(values).every((value) => value !== '');
              const isButtonDisabled = !canSubmit || !isFieldsValid || !isDirty || !isAllFieldsFilled;
              return (
                <Button
                  label="Войти"
                  type="submit"
                  loading={pendingLogin}
                  disabled={isButtonDisabled}
                  className="sm:w-21rem w-full mt-3 mb-2"
                />
              );
            }}
          />
          <Button
            label="Зарегистрироваться"
            onClick={() => {
              routes.signup.open();
            }}
            severity="secondary"
            className="sm:w-21rem w-full h-2rem mb-4"
          />
          {/*<span className="text-center mb-2">Войти с помощью</span>
        На данный момент данный блок не нужен
        <div className="flex justify-content-center gap-3">
           TODO: Сделать событие клика на иконку
          <img alt="" className="border-circle border-1 w-2rem h-2rem" />
          <img alt="" className="border-circle border-1 w-2rem h-2rem" />
          <img alt="" className="border-circle border-1 w-2rem h-2rem" />
          <img alt="" className="border-circle border-1 w-2rem h-2rem" />
          <img alt="" className="border-circle border-1 w-2rem h-2rem" />
        </div>*/}
        </form>
      </div>
    </div>
  );
};
