import ReactDOM from 'react-dom/client';
import 'primeflex/primeflex.css';
import './index.scss';
import { PrimeReactProvider } from 'primereact/api';
import { createGate, useGate } from 'effector-react';
import { $authorized, checkViewerQuery } from './shared/auth';
import { sample } from 'effector';
import { not } from 'patronum';
import { ProfileMenu } from './widgets/profile-menu';

const ProfileWidgetGate = createGate();

sample({
  clock: ProfileWidgetGate.open,
  filter: not($authorized),
  fn: () => ({}),
  target: checkViewerQuery.start,
});

export default function Widget() {
  useGate(ProfileWidgetGate);
  const value = {
    zIndex: {
      menu: 1031,
    },
    autoZIndex: true,
  };

  return (
    <PrimeReactProvider value={value}>
      <ProfileMenu />
    </PrimeReactProvider>
  );
}

ReactDOM.createRoot(document.getElementById('profile') as HTMLElement).render(<Widget />);
