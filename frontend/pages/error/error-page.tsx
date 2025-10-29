import { ErrorView } from '~/shared/ui/error';
import { useUnit } from 'effector-react';
import { $error } from './model';
import { Header } from '~/widgets/header';
import { Footer } from '~/shared/ui/layout/footer';

export const ErrorPage = () => {
  const props = useUnit($error);
  return (
    <div className="flex flex-column justify-content-between" style={{ height: '100svh' }}>
      <Header />
      <ErrorView {...props} />
      <Footer />
    </div>
  );
};
