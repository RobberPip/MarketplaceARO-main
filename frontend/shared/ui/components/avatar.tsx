import React, { useState, useCallback, useRef } from 'react';
import { Avatar } from 'primereact/avatar';
import { FileUpload } from 'primereact/fileupload';
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { Divider } from 'primereact/divider';
import { Slider } from 'primereact/slider';
import Cropper, { Area } from 'react-easy-crop';
import { useUnit } from 'effector-react';

const MAX_FILE_SIZE = 10 * 1024 * 1024;
type CustomAvatarProps = {
  avatarUrl?: string;
  onFileSelect?: (file: File | null) => void;
  updateMutation?: any;
  autoUpload?: boolean;
  className?: string;
  disabled?: boolean;
  previewUrl?: string | null;
};

// Функция для обрезки изображения через canvas
async function getCroppedImg(imageSrc: string, crop: Area) {
  const image = new Image();
  image.src = imageSrc;
  // biome-ignore lint/suspicious/noAssignInExpressions: <explanation>
  await new Promise((resolve) => (image.onload = resolve));

  const canvas = document.createElement('canvas');
  canvas.width = crop.width;
  canvas.height = crop.height;
  const ctx = canvas.getContext('2d');

  if (!ctx) throw new Error('Cannot get canvas context');

  ctx.drawImage(image, crop.x, crop.y, crop.width, crop.height, 0, 0, crop.width, crop.height);

  return new Promise<File>((resolve) => {
    canvas.toBlob((blob) => {
      if (!blob) return resolve(null as any);
      resolve(new File([blob], 'avatar.png', { type: 'image/png' }));
    }, 'image/png');
  });
}

export const CustomAvatar = ({
  avatarUrl,
  onFileSelect,
  updateMutation,
  autoUpload = true,
  className,
  disabled = false,
  previewUrl,
}: CustomAvatarProps) => {
  const { start: upload } = useUnit(updateMutation ?? { start: undefined });
  const [imageSrc, setImageSrc] = useState<string | null>(null);
  const [dialogMode, setDialogMode] = useState<'none' | 'error' | 'crop'>('none');
  const [crop, setCrop] = useState({ x: 0, y: 0 });
  const [zoom, setZoom] = useState(1);
  const [croppedAreaPixels, setCroppedAreaPixels] = useState<Area | null>(null);
  const fileUploadRef = useRef<FileUpload | null>(null);

  const handleChooseAnother = () => {
    const input = fileUploadRef.current?.getInput?.();
    if (input) input.click();
  };

  const handleFileSelect = (event: any) => {
    const file = event.files?.[0] ?? null;
    if (file) {
      if (file.size > MAX_FILE_SIZE || file.type !== 'image/jpeg') {
        setDialogMode('error');
        return;
      }
      const reader = new FileReader();
      reader.onload = () => {
        setImageSrc(reader.result as string);
        setDialogMode('crop');
      };
      reader.readAsDataURL(file);
    }
  };

  const onCropComplete = useCallback((_: Area, croppedAreaPixels: Area) => {
    setCroppedAreaPixels(croppedAreaPixels);
  }, []);

  const handleSave = async () => {
    if (!imageSrc || !croppedAreaPixels) return;

    const croppedFile = await getCroppedImg(imageSrc, croppedAreaPixels);

    if (onFileSelect) onFileSelect(croppedFile);
    if (autoUpload && typeof upload === 'function' && croppedFile) {
      upload({ files: [croppedFile], options: {} });
    }

    setDialogMode('none');
  };

  return (
    <div className={`relative ${className}`}>
      <FileUpload
        ref={fileUploadRef}
        accept="image/jpeg"
        customUpload
        auto={false}
        onSelect={handleFileSelect}
        disabled={disabled}
        contentStyle={{ display: 'none' }}
        chooseOptions={{
          icon: '',
          iconOnly: true,
          className: 'border-none absolute custom-choose-btn p-button-rounded p-button-outlined w-5rem h-5rem',
          style: { backgroundColor: 'transparent' },
        }}
        pt={{
          chooseIcon: { style: { display: 'none' } },
        }}
        headerTemplate={({ chooseButton }) => (
          <>
            {chooseButton}
            <Avatar
              shape="circle"
              icon="icon icon-camera"
              size="xlarge"
              pt={{
                root: { className: 'flex-shrink-0 cursor-pointer border-1 w-5rem h-5rem align-items-end' },
                icon: { className: 'text-base mb-2' },
              }}
              image={previewUrl ?? avatarUrl ?? undefined}
            />
          </>
        )}
      />
      <Dialog
        header="Выберите фото"
        headerClassName="py-4"
        visible={dialogMode !== 'none'}
        style={{ width: '544px' }}
        onHide={() => setDialogMode('none')}
      >
        {dialogMode === 'error' && (
          <div className="relative w-full flex h-18rem flex justify-content-center bg-gray-200">
            <span className="flex flex-column align-items-center text-center justify-content-center ">
              <span className="flex icon-10xl icon-camera-x mb-4" />
              Изображение должно быть в формате JPEG и весить
              <br /> не больше 10 мб
            </span>
          </div>
        )}

        {dialogMode === 'crop' && imageSrc && (
          <div className="relative w-full h-18rem overflow-hidden">
            <Cropper
              image={imageSrc}
              crop={crop}
              zoom={zoom}
              aspect={1}
              cropShape="round"
              showGrid={false}
              onCropChange={setCrop}
              onZoomChange={setZoom}
              onCropComplete={onCropComplete}
            />
          </div>
        )}

        <div className="flex align-items-center gap-3">
          <Slider
            value={zoom}
            min={1}
            max={3}
            step={0.01}
            onChange={(e) => {
              if (typeof e.value === 'number') setZoom(e.value);
            }}
            className="w-full my-4"
          />
        </div>

        <Divider className="my-4" />
        <div className="flex justify-content-between mb-4">
          <Button label="Отменить" severity="secondary" className="w-14rem" onClick={() => setDialogMode('none')} />

          {dialogMode === 'error' ? (
            <Button label="Выбрать другое фото" className="w-14rem" onClick={handleChooseAnother} />
          ) : (
            <Button label="Сохранить" className="w-14rem" onClick={handleSave} />
          )}
        </div>
      </Dialog>
    </div>
  );
};
