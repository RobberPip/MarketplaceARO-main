import { Link } from 'atomic-router-react';
import { Menubar } from 'primereact/menubar';
import { Toolbar } from 'primereact/toolbar';
import { routes } from '~/shared/routing';

export const Footer = () => {
  return (
    <Toolbar
      className="px-7 py-4 surface-700 border-none border-noround text-sm text-white"
      start={<span>© 2023-2025 ООО «АРО»</span>}
      center={<span className="text-white">Нашли ошибку в перечне ПО? Напишите нам на update@arogroup.ru</span>}
      end={
        <Link to={routes.docs} target="_blank" className="underline text-white">
          Правовая документация
        </Link>
      }
    />
  );
};
