import React, { useRef, useEffect } from 'react';
import { FileUpload } from 'primereact/fileupload';
import { useUnit } from 'effector-react';
import { createFileUploadModel } from './model';

type InitialFileUrl = string | { objectURL: string };
type Props<FormType extends { setFieldValue: (...args: any[]) => void }> = {
  className?: string;
  initialFileUrls?: InitialFileUrl[];
  form: FormType;
  fieldName: string;
};

export const ImageFileUpload = <FormType extends { setFieldValue: (...args: any[]) => void }>({
  className,
  initialFileUrls = [],
  form,
  fieldName,
}: Props<FormType>) => {
  const model = React.useMemo(() => createFileUploadModel(), []);
  const fileUploadRef = useRef<FileUpload | null>(null);
  const initialFiles = useUnit(model.$initialFiles);
  const resetFiles = model.resetInitialFiles;
  const loadFiles = model.loadFilesFx;
  const currentFilesRef = useRef<File[]>([]);

  useEffect(() => {
    async function initializeFiles() {
      const urlsToLoad = initialFileUrls
        .map((urlOrObj) => (typeof urlOrObj === 'string' ? urlOrObj : urlOrObj.objectURL))
        .filter((url) => typeof url === 'string' && !url.startsWith('blob:'));

      if (urlsToLoad.length > 0 && initialFiles.length === 0) {
        loadFiles(urlsToLoad);
      }

      if (initialFiles.length > 0) {
        currentFilesRef.current = initialFiles;
        form.setFieldValue(fieldName, initialFiles);

        if (fileUploadRef.current) {
          fileUploadRef.current.clear();
          const filesWithObjectURL = initialFiles.map((file) =>
            Object.assign(file, { objectURL: URL.createObjectURL(file) }),
          );
          fileUploadRef.current.setFiles(filesWithObjectURL);
        }
      }
    }
    initializeFiles();
  }, [initialFileUrls, initialFiles, loadFiles, form, fieldName]);

  const emptyTemplate = (
    <div
      onClick={() => fileUploadRef.current?.getInput?.()?.click()}
      className="h-7rem flex flex-column justify-content-center align-items-center text-color-secondary cursor-pointer"
    >
      <p className="m-0 mb-1">Перетащите изображение сюда для загрузки</p>
      <span className="icon-3xl icon-camera-plus" />{' '}
    </div>
  );
  const MAX_FILE_SIZE = 2.5 * 1024 * 1024; // 2.5 МБ
  const MAX_FILES_COUNT = 10;
  const onSelect = (event: any) => {
    const allowedTypes = ['image/png', 'image/jpeg'];
    const invalidFiles = event.files.filter((file: File) => !allowedTypes.includes(file.type));
    if (invalidFiles.length > 0) {
      alert('Only PNG and JPEG files are allowed.');
      currentFilesRef.current = [];
      fileUploadRef.current?.clear();
      resetFiles();
      form.setFieldValue(fieldName, []);
      return;
    }
    const oversizedFiles = event.files.filter((file: File) => file.size > MAX_FILE_SIZE);
    if (oversizedFiles.length > 0) {
      alert('Максимальный размер файла — 2.5 МБ.');
      return;
    }
    if (currentFilesRef.current.length + event.files.length > MAX_FILES_COUNT) {
      alert('Максимальное количество файлов — 10.');
      return;
    }
    const newFiles = event.files.filter(
      (newFile: File) =>
        !currentFilesRef.current.some(
          (existingFile) =>
            (existingFile as File & { objectURL?: string }).objectURL ===
            (newFile as File & { objectURL?: string }).objectURL,
        ),
    );
    currentFilesRef.current = [...currentFilesRef.current, ...newFiles];
    form.setFieldValue(fieldName, currentFilesRef.current);
    if (fileUploadRef.current) {
      fileUploadRef.current.clear();
      const filesWithObjectURL = currentFilesRef.current.map((file) =>
        Object.assign(file, { objectURL: URL.createObjectURL(file) }),
      );
      fileUploadRef.current.setFiles(filesWithObjectURL);
    }
  };

  const onRemove = (event: any) => {
    const removedFile = event.file;
    currentFilesRef.current = currentFilesRef.current.filter(
      (file) => !((file as File & { objectURL?: string }).objectURL === removedFile.objectURL),
    );
    form.setFieldValue(fieldName, currentFilesRef.current);
    resetFiles();
  };

  return (
    <FileUpload
      ref={fileUploadRef}
      accept="image/png, image/jpeg"
      multiple
      onRemove={onRemove}
      className={`p-0 ${className}`}
      emptyTemplate={emptyTemplate}
      onSelect={onSelect}
      contentClassName="border-2 border-dashed border-gray-400 py-2"
      pt={{
        content: { style: { borderRadius: '8px' } },
        fileName: { className: 'text-sm' },
        fileSize: { className: 'mt-2 text-sm text-color-secondary' },
        thumbnail: { style: { height: '70px', width: '100px' } },
        removeButton: {
          root: {
            className: 'p-0 absolute',
            style: {
              left: '104px',
              transform: 'translateY(-190%)',
              width: '24px',
              height: '24px',
              backgroundColor: '#898989',
              border: '1px solid #898989',
              color: '#ffffff',
            },
          },
        },
        progressbar: { root: { className: 'hidden' } },
        badge: { root: { className: 'hidden' } },
        cancelIcon: { className: 'hidden' },
        buttonbar: { className: 'hidden' },
      }}
    />
  );
};
