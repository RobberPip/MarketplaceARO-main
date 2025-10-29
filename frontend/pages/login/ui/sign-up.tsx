import { useUnit } from 'effector-react';
import { Button } from 'primereact/button';
import { Checkbox } from 'primereact/checkbox';
import { InputText } from 'primereact/inputtext';
import { Message } from 'primereact/message';
import { Password } from 'primereact/password';
import { $emailError, $passwordError, registrationForm, registrationMutation } from '../model';
import { PasswordError } from '~/shared/ui/components/index.ts';
import { routes } from '~/shared/routing';
import { Link } from 'atomic-router-react';

export const SignUp = () => {
  const { pending } = useUnit(registrationMutation);
  const [emailError] = useUnit([$emailError]);
  const { extendedForm: form } = useUnit(registrationForm);

  const [passwordError] = useUnit([$passwordError]);

  return (
    <div className="flex flex-column align-items-center my-8">
      <h1 className="mt-7 mb-4 max-w-29rem font-medium text-2xl">Регистрация</h1>
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
        <article className="flex flex-column w-full sm:w-21rem justify-content-center gap-2">
          <div className="text-color-secondary flex flex-column gap-2">
            <section className="flex gap-2">
              <form.Field
                name="data.firstName"
                children={(field) => (
                  <div className="flex flex-column w-full gap-1">
                    <label htmlFor="name">Имя*</label>
                    <InputText
                      id={field.name}
                      name={field.name}
                      value={field.state.value}
                      onBlur={field.handleBlur}
                      onChange={(e) => field.handleChange(e.target.value)}
                      invalid={field.state.meta.errors.length > 0}
                      className="w-full"
                      placeholder="Введите"
                    />
                  </div>
                )}
              />
              <form.Field
                name="data.lastName"
                children={(field) => (
                  <div className="flex flex-column w-full gap-1">
                    <label htmlFor="surname">Фамилия*</label>
                    <InputText
                      id={field.name}
                      name={field.name}
                      value={field.state.value}
                      onBlur={field.handleBlur}
                      onChange={(e) => field.handleChange(e.target.value)}
                      invalid={field.state.meta.errors.length > 0}
                      className="w-full"
                      placeholder="Введите"
                      autoComplete="username"
                    />
                  </div>
                )}
              />
            </section>
            <section className="flex flex-column gap-1">
              <form.Field
                name="email"
                children={(field) => (
                  <label htmlFor="email">
                    Почта*
                    <InputText
                      id={field.name}
                      name={field.name}
                      value={field.state.value}
                      onBlur={field.handleBlur}
                      onChange={(e) => field.handleChange(e.target.value)}
                      placeholder="Введите"
                      invalid={!!emailError || field.state.meta.errors.length > 0}
                      className="w-full"
                    />
                    {field.state.meta.errors.length > 0 && (
                      <Message className="w-full mt-1" severity="error" text={field.state.meta.errors.join('\n')} />
                    )}
                    {!!emailError && (
                      <Message
                        severity="error"
                        text={
                          <div>
                            Пользователь с такой почтой уже существует <br />
                            <span className="text-primary flex justify-content-center">
                              <Link to={routes.signin} className="text-primary">
                                Войти
                              </Link>
                              &nbsp;в профиль?
                            </span>
                          </div>
                        }
                      />
                    )}
                  </label>
                )}
              />
            </section>
            <section className="flex flex-column gap-1">
              <div className="flex">
                <label htmlFor="password">Пароль*</label>
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
                      invalid={!!passwordError || field.state.meta.errors.length > 0}
                      feedback={false}
                      toggleMask
                      placeholder="Придумайте пароль"
                      autoComplete="new-password"
                    />
                    {field.state.meta.errors.length > 0 && (
                      <PasswordError passwordError={field.state.meta.errors.join('/n')} />
                    )}
                  </>
                )}
              />
            </section>
          </div>
        </article>
        <article className="flex gap-3 justify-center w-full sm:w-21rem my-3">
          <form.Field
            name="checkedAgreements"
            children={(field) => (
              <Checkbox
                className="ml-1"
                id={field.name}
                name={field.name}
                onBlur={field.handleBlur}
                onChange={(e) => field.handleChange(!!e.target.checked)}
                checked={Boolean(field.state.value)}
              />
            )}
          />
          <span className="text-xs">
            Я подтверждаю, что даю согласие на{' '}
            <Link to={routes.docs} target="_blank" query={{ name: 'personal-data' }} className="underline text-primary">
              обработку персональных данных
            </Link>
          </span>
        </article>
        <article className="flex gap-3 justify-center w-full sm:w-21rem">
          <form.Field
            name="checkedRules"
            children={(field) => (
              <Checkbox
                className="ml-1"
                id={field.name}
                name={field.name}
                onBlur={field.handleBlur}
                onChange={(e) => field.handleChange(!!e.target.checked)}
                checked={Boolean(field.state.value)}
              />
            )}
          />
          <span className="text-xs ">
            Я подтверждаю, что ознакомлен и согласен
            <br /> с{' '}
            <Link to={routes.docs} target="_blank" query={{ name: 'terms' }} className="underline text-primary">
              Пользовательским соглашением
            </Link>{' '}
            и{' '}
            <Link to={routes.docs} target="_blank" query={{ name: 'privacy' }} className="underline text-primary">
              Политикой конфиденциальности
            </Link>
          </span>
        </article>
        <form.Subscribe
          selector={(state) => state.values}
          children={(values) => {
            const { email, password, data, checkedAgreements, checkedRules } = values ?? {};
            const isAllFieldsFilled =
              [email, password, data?.firstName, data?.lastName].every(
                (v) => typeof v === 'string' && v.trim() !== '',
              ) &&
              checkedAgreements === true &&
              checkedRules === true;
            const isButtonDisabled = !isAllFieldsFilled;

            return (
              <Button
                label="Зарегистрироваться"
                loading={pending}
                type="submit"
                className="sm:w-21rem w-full mt-4 h-2rem"
                disabled={isButtonDisabled}
              />
            );
          }}
        />
      </form>
      <span className="mt-4">
        Уже есть профиль?{' '}
        <Link to={routes.signin} className="text-primary underline">
          Войти
        </Link>
      </span>
    </div>
  );
};
