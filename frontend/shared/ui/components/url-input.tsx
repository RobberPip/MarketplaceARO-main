import { InputText } from 'primereact/inputtext';
import { ChangeEvent, FocusEvent } from 'react';
type UrlInputProps = {
  field: any;
  placeholder?: string;
  invalid?: boolean;
};
function ensureProtocol(val: string): string {
  if (!val) return val;
  if (/^https?:\/\//i.test(val)) return val;
  if (/^\/\//.test(val)) return `https:${val}`;
  if (/^[a-z0-9.-]+\.[a-z]{2,}.*$/i.test(val)) return `https://${val}`;
  return `https://${val}`;
}

export const UrlInput: React.FC<UrlInputProps> = ({ field, placeholder = 'https://', invalid }) => {
  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const raw = e.target.value;
    field.handleChange(raw);
  };

  const handleBlur = (e: FocusEvent<HTMLInputElement>) => {
    const raw = e.target.value;
    if (raw && raw.trim() !== '') {
      const corrected = ensureProtocol(raw);
      field.handleChange(corrected);
    }

    field.handleBlur();
  };

  const value = field.state.value ?? '';
  return (
    <InputText
      placeholder={placeholder}
      id={field.name}
      name={field.name}
      value={value}
      invalid={!!(field.state.meta.errors?.length > 0)}
      onBlur={handleBlur}
      onChange={handleChange}
      aria-invalid={!!(field.state.meta.errors?.length > 0)}
    />
  );
};
