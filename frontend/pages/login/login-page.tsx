import { useUnit } from 'effector-react';
import { Layout } from '~/shared/ui/layout';

import { Button } from 'primereact/button';
import { Header } from '~/widgets/header';
import { $emailSent, $loginStep, loginRoute, registrationForm, resetpassRoute, sendConfirmedEmail } from './model';
import { SignIn } from './ui/sign-in';
import { SignUp } from './ui/sign-up';
import { SignUpInfo } from './ui/sign-up-info';
import { useInterval } from 'primereact/hooks';
import { useState } from 'react';
import { Link } from 'atomic-router-react';
import { routes } from '~/shared/routing';
import { ForgotPass, ResetPass } from './ui/reset-pass';
import { formatTime } from '~/shared/lib/format';
import { $viewer } from '~/shared/auth';

export const LoginPage = () => {
  const [isLogin, isReset, emailSent, step] = useUnit([
    loginRoute.$isOpened,
    resetpassRoute.$isOpened,
    $emailSent,
    $loginStep,
  ]);
  return (
    <Layout className="bg-white" header={<Header />}>
      {emailSent ? (
        <EmailSent />
      ) : step === 'reset' ? (
        <ResetPass />
      ) : isReset ? (
        <ForgotPass />
      ) : isLogin ? (
        <SignIn />
      ) : step === 'info' ? (
        <SignUpInfo />
      ) : (
        <SignUp />
      )}
    </Layout>
  );
};

function EmailSent() {
  const email = useUnit(registrationForm.$values.map((q) => q?.email));
  const sendConfirmedEmailCall = useUnit(sendConfirmedEmail);
  // Timers send email
  const [timeLeft, setTimeLeft] = useState(60);
  const [buttonDisabled, setButtonDisabled] = useState(true);

  useInterval(() => {
    if (timeLeft > 0) {
      setTimeLeft(timeLeft - 1);
    } else {
      setButtonDisabled(false);
    }
  }, 1000);
  return (
    <article className="flex flex-column align-items-center mt-8">
      <h1 className="text-4xl text-center mt-7 mb-4">
        Проверьте свою <br /> электронную почту
      </h1>
      <p className="text-center line-height-3 mt-0">
        Для завершения регистрации мы отправили <br /> ссылку для входа на вашу электронную почту: <br />
        {email} <br />
        Убедитесь, что вам пришло письмо.
      </p>
      {timeLeft > 0 && (
        <>
          <div className="mb-2">Отправить повторно через</div>
          <div className="mb-4 font-semibold text-base">{formatTime(timeLeft)}</div>
        </>
      )}
      <Button
        label="Повторно отправить ссылку"
        className="sm:w-21rem w-full mb-4 mb-4"
        size="small"
        severity="secondary"
        disabled={buttonDisabled}
        onClick={() => {
          sendConfirmedEmailCall();
          setTimeLeft(60);
          setButtonDisabled(true);
        }}
      />
      <span className="mb-3">
        Уже прошли проверку?{' '}
        <Link to={routes.signin} className="text-primary">
          Войти
        </Link>
      </span>
      <span>Возникли проблемы с регистрацией?</span>
      <span>
        Напишите нам нам почту <span className="underline">support@arogroup.ru</span>
      </span>
    </article>
  );
}
