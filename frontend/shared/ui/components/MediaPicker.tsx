import { Avatar } from 'primereact/avatar';
import { Image } from 'primereact/image';
import { useRef, useState } from 'react';

interface MediaPickerProps {
  maxImages?: number;
  maxVideos?: number;
  maxVideoDuration?: number;
  multiple?: boolean;
  accept?: string;
  onChange?: (files: File[]) => void;
}

export default function MediaPicker({
  maxImages = 10,
  maxVideos = 3,
  maxVideoDuration = 600, // 10 минут в секундах
  multiple = true,
  accept = 'image/*,video/*',
  onChange,
}: MediaPickerProps) {
  const [files, setFiles] = useState<File[]>([]);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFiles = Array.from(event.target.files || []);
    const images = files.filter((f) => f.type.startsWith('image'));
    const videos = files.filter((f) => f.type.startsWith('video'));

    for (const file of selectedFiles) {
      if (file.type.startsWith('image') && images.length < maxImages) {
        images.push(file);
      } else if (file.type.startsWith('video') && videos.length < maxVideos) {
        const videoDuration = await getVideoDuration(file);
        if (videoDuration <= maxVideoDuration) {
          videos.push(file);
        }
      }
    }

    const updatedFiles = [...images, ...videos];
    setFiles(updatedFiles);
    if (onChange) {
      onChange(updatedFiles);
    }
  };

  const getVideoDuration = (file: File): Promise<number> => {
    return new Promise((resolve) => {
      const video = document.createElement('video');
      video.preload = 'metadata';
      video.onloadedmetadata = () => {
        window.URL.revokeObjectURL(video.src);
        resolve(video.duration);
      };
      video.src = URL.createObjectURL(file);
    });
  };

  return (
    <div className="grid gap-4 justify-center items-center">
      {files.map((file: File) => (
        <Image src={URL.createObjectURL(file)} alt="Image" width="240" height="160" preview />
      ))}
      <Avatar
        icon="icon icon-camera icon-2xl"
        size="xlarge"
        pt={{
          root: {
            className:
              'flex align-items-center justify-content-center w-15rem h-10rem rounded cursor-pointer bg-gray-100',
          },
        }}
        onClick={() => fileInputRef.current?.click()}
      />
      <input
        type="file"
        ref={fileInputRef}
        className="hidden"
        accept={accept}
        multiple={multiple}
        onChange={handleFileSelect}
      />
    </div>
  );
}
