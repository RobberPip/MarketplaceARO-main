import React, { useMemo } from 'react';
import { FormApi, useField } from '@tanstack/react-form';
import clsx from 'clsx';

export enum HeadingLevel {
  H1 = 'text-3xl',
  H2 = 'text-2xl',
  H3 = 'text-xl',
}

interface InputProps {
  name: string;
  form?: FormApi<any>;
  value?: string;
  onChange?: (value: string) => void;
  placeholder?: string;
  headingLevel?: HeadingLevel;
  errors?: string[];
}

export const NameInput: React.FC<InputProps> = ({
  name,
  form,
  value,
  onChange,
  placeholder = 'Введите наименование',
  headingLevel = HeadingLevel.H1,
  errors = [],
}) => {
  const field = form ? useField({ name, form }) : null;

  const { inputValue, handleChange, allErrors } = useMemo(() => {
    if (field) {
      return {
        inputValue: field.state.value,
        handleChange: field.handleChange,
        allErrors: field.state.meta.errors,
      };
    }

    return { inputValue: value, handleChange: onChange, allErrors: errors };
  }, [field, value, onChange, errors]);

  const hasError = allErrors.length > 0;

  return (
    <div className="flex flex-col w-full">
      <input
        id={name}
        type="text"
        placeholder={placeholder}
        value={inputValue || ''}
        onChange={(e) => handleChange?.(e.target.value)}
        className={clsx(
          'border-none outline-none',
          'w-full bg-transparent font-medium',
          'placeholder-gray-400',
          inputValue && 'text-gray-900 hover:text-gray-700',
          hasError && 'text-red-600 placeholder-red-400',
          headingLevel,
        )}
      />
      {hasError && (
        <div className="text-red-500 text-sm mt-1">
          {allErrors.map((err, index) => (
            <p key={index}>{err}</p>
          ))}
        </div>
      )}
    </div>
  );
};
