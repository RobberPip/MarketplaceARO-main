import { createStore, createEvent, createEffect } from 'effector';

export const fetchFileFx = createEffect(async (url: string): Promise<File> => {
  const response = await fetch(`${window.location.origin}${url}`);
  const blob = await response.blob();
  const filename = url.split('/').pop() || 'file';
  return new File([blob], filename, { type: blob.type });
});

export const loadFilesFx = createEffect(async (urls: string[]): Promise<File[]> => {
  if (urls.length === 0) return [];
  return await Promise.all(urls.map(fetchFileFx));
});

export const resetInitialFiles = createEvent<void>();

export function createFileUploadModel() {
  const $initialFiles = createStore<File[]>([]);
  $initialFiles.on(loadFilesFx.doneData, (_, files) => files);
  $initialFiles.on(resetInitialFiles, () => []);

  return { $initialFiles, loadFilesFx, resetInitialFiles };
}
