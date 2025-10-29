import { attach, createEvent, createStore, sample } from 'effector';
import { createGate } from 'effector-react';
import { combineEvents } from 'patronum';
import { Toast, ToastMessage } from 'primereact/toast';
import { MutableRefObject } from 'react';

export const showToast = createEvent<ToastMessage>();

export const toastGate = createGate<MutableRefObject<Toast | null>>();
const $queue = createStore<ToastMessage[]>([]);

export const showToastFx = attach({
  source: { ref: toastGate.state, messages: $queue },
  effect: ({ ref, messages }) => {
    for (const toast of messages) {
      ref.current?.show(toast);
    }
  },
});

$queue.on(showToast, (q, t) => [...q, t]);
sample({
  clock: [
    // Preserved messages emitted before <Notifications /> mounts
    combineEvents([toastGate.open, showToast]),
    // Regular messages
    sample({ clock: showToast, filter: toastGate.state.map(({ current }) => !!current) }),
  ],
  source: $queue,
  target: showToastFx,
});
sample({
  clock: showToastFx.done,
  target: $queue.reinit,
});
