import { useUnit } from 'effector-react';
import { ReactNode, useEffect, useMemo, useRef } from 'react';
import { profileQuery } from '~/pages/profile/model.ts';
import { router } from '~/shared/routing';
import { Layout } from '~/shared/ui/layout';
import { Header } from '~/widgets/header';
import { ProfileNavbar } from '~/widgets/profile-navbar';
import { Footer } from '../footer';
import { ProgressSpinner } from 'primereact/progressspinner';
import { ErrorView } from '../../error';
import { DocsNavbar } from '~/widgets/docs-navbar';

export const DocsLayout: React.FC<{ children: ReactNode }> = ({ children }) => {
  const path = useUnit(router.$query);
  const containerRef = useRef<HTMLDivElement | null>(null);

  const { error, pending } = useUnit(profileQuery);

  // scroll hack
  // biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
  useEffect(() => {
    window.scrollTo(0, 0);
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

  return (
    <Layout
      mainClassName="main-content-profile"
      mainStyle={{ maxWidth: '976px' }}
      sidebarClassName={'p-component w-23rem bg-white border-right-1 border-gray-200'}
      header={<Header />}
      sidebar={<DocsNavbar />}
    >
      <div ref={containerRef} className="mt-4">
        {pageView}
      </div>
    </Layout>
  );
};
