import { RouterProvider } from 'atomic-router-react';
import { PrimeReactProvider } from 'primereact/api';
import { useEffect } from 'react';
import { Pages } from '~/pages';
import { appStarted } from '~/shared/config';
import Notifications from '~/shared/notifications/ui';
import { router } from '~/shared/routing';
import { appLoaded } from './model';

appStarted();

export function App() {
  useEffect(() => {
    appLoaded();
  }, []);
  return (
    <RouterProvider router={router}>
      <PrimeReactProvider>
        <Notifications />
        <Pages />
      </PrimeReactProvider>
    </RouterProvider>
  );
}
