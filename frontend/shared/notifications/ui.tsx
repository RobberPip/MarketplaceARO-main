import { Toast } from 'primereact/toast';
import { useRef } from 'react';
import { toastGate } from './model';
import { useGate } from 'effector-react';

export default function Notifications() {
  const toast = useRef<Toast | null>(null);
  useGate(toastGate, toast);

  return (
    <div className="card flex justify-content-center">
      <Toast
        ref={toast}
        position="top-center"
        pt={{
          root: { style: { paddingTop: '52px' } },
        }}
      />
    </div>
  );
}
