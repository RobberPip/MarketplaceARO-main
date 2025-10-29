import { FC } from 'react';

export const ErrorView: FC<{ code?: string; title?: string; description?: string }> = ({
  code,
  title,
  description,
}) => {
  return (
    <div className="flex flex-column justify-content-center align-items-center">
      <h1 className="mb-4 mt-0" style={{ fontSize: '5rem' }}>
        {code}
      </h1>
      <h2 className="text-2xl mb-3 mt-0">{title}</h2>
      <span className="w-24rem text-sm text-center mb-4">{description}</span>
      <a href="/" className="p-button no-underline">
        В реестр
      </a>
    </div>
  );
};
