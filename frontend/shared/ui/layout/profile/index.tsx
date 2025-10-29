import { useUnit } from 'effector-react';
import { ReactNode, useEffect, useMemo, useRef } from 'react';
import { profileQuery } from '~/pages/profile/model.ts';
import { router, routes } from '~/shared/routing';
import { Layout } from '~/shared/ui/layout';
import { Header } from '~/widgets/header';
import { ProfileNavbar } from '~/widgets/profile-navbar';
import { Footer } from '../footer';
import { ProgressSpinner } from 'primereact/progressspinner';
import { ErrorView } from '../../error';
import { ProductNavbar } from '~/widgets/product-navbar';

export const ProfileLayout: React.FC<{ children: ReactNode }> = ({ children }) => {
  const path = useUnit(router.$path);
  const containerRef = useRef<HTMLDivElement | null>(null);
  const isOpenedProduct = useUnit(routes.profile['products-new'].$isOpened);

  const { error, pending } = useUnit(profileQuery);

  // scroll hack
  // biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
  useEffect(() => {
    if (containerRef.current) {
      containerRef.current.parentElement!.scrollTop = 0;
    }
  }, [path]);

  const pageView = useMemo(() => {
    if (pending) {
      console.log('PENDING');

      return (
        <div className="card flex justify-content-center align-items-center h-screen">
          <ProgressSpinner />
        </div>
      );
    }

    return children;
  }, [pending, children]);

  if (error) {
    return (
      <Layout mainClassName="flex flex-column align-items-center justify-content-center">
        {'status' in error && error.status === 403 ? (
          <ErrorView
            title="Доступ запрещён"
            code="403"
            description="У вас нет прав для просмотра этой страницы.
Попробуйте войти под другим профилем"
          />
        ) : (
          <ErrorView
            title="Страница не доступна"
            code="500"
            description="Страница временно недоступна.
Обновите страницу через несколько минут.
Если проблема не решится, напишите в техподдержку
update@arogroup.ru"
          />
        )}
      </Layout>
    );
  }
  // biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
  const sidebar = useMemo(() => {
    if (isOpenedProduct) {
      return <ProductNavbar />;
    }
    return <ProfileNavbar />;
  }, [path]);
  return (
    <Layout
      header={<Header />}
      mainClassName="main-content-profile"
      // mainStyle={{ maxWidth: '976px' }}
      sidebarClassName={'p-component bg-white border-right-1 border-gray-200'}
      sidebarStyle={{ width: '368px' }}
      sidebar={sidebar}
      footer={<Footer />}
    >
      <div ref={containerRef} className="mt-4">
        {pageView}
      </div>
    </Layout>
  );
};
