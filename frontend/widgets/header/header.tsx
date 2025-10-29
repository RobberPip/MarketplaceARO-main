import { Link } from 'atomic-router-react';
import { useUnit } from 'effector-react';
import { Menubar } from 'primereact/menubar';
import { $authorized } from '~/shared/auth';
import { routes } from '~/shared/routing';
import { ProfileMenu } from '../profile-menu/profile-menu';

export const Header: React.FC = () => {
  const isAuth = useUnit($authorized);

  return (
    <Menubar
      className="pl-7 py-2 pr-6"
      style={{ height: '72px' }}
      start={
        <Link
          target="_blank"
          to="https://arogroup.ru/find-us"
          className="icon-4xl icon-aro-logo cursor-pointer bg-gray-900"
          rel="noreferrer"
        />
      }
      model={[
        { label: 'Главная', url: '/' },
        { label: 'Реестр', url: '/softwares' },
        { label: 'Коннектор', url: 'https://kb.arogroup.ru/spaces/PUB/overview' },
        { label: 'Контакты', url: '/#contact-block' },

        {
          template: (
            <>
              {/*TODO owerflow*/}
              {isAuth ? (
                <ProfileMenu />
              ) : (
                <Link className="p-menuitem-link text-color-secondary" to={routes.signin}>
                  Войти
                </Link>
              )}
            </>
          ),
        },
      ]}
      pt={{
        root: { className: 'justify-content-between' },
        content: {
          className: 'cursor-pointer',
          style: { backgroundColor: 'transparent' },
        },
        label: { className: 'cursor-pointer text-500 hover:text-900' },
      }}
    />
  );
};
