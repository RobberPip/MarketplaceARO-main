import { Message } from 'primereact/message';

export const PasswordError = ({ passwordError }: any) => {
  // Types errors
  const hasLetterError = passwordError?.toLowerCase().includes('passwordrequiresletter');
  const hasDigitError = passwordError?.toLowerCase().includes('passwordrequiresdigit');
  const hasLengthError = passwordError?.toLowerCase().includes('passwordtooshort');
  const hasSymbolError = passwordError?.toLowerCase().includes('passwordrequiresuniquechars');

  if (!hasLetterError && !hasDigitError && !hasLengthError && !hasSymbolError) {
    return null;
  }

  // TODO Change on filled icons
  const content = (
    <div className="flex flex-column">
      <div className="flex items-center">
        <span className={`icon-2xl ${hasLetterError ? 'icon-point' : 'icon-check'} bg-primary mb-2`} />
        <span className="text-primary m-0 ml-2">Как минимум одна буква</span>
      </div>
      <div className="flex items-center mb-2">
        <span className={`icon-2xl ${hasDigitError ? 'icon-point' : 'icon-check'} bg-primary`} />
        <span className="text-primary m-0 ml-2">Как минимум одна цифра</span>
      </div>
      <div className="flex items-center mb-2">
        <span className={`icon-2xl ${hasLengthError ? 'icon-point' : 'icon-check'} bg-primary`} />
        <span className="text-primary m-0 ml-2">Как минимум 8 символов</span>
      </div>
      <div className="flex items-center">
        <span className={`icon-2xl ${hasSymbolError ? 'icon-point' : 'icon-check'} bg-primary`} />
        <span className="text-primary m-0 ml-2">Как минимум 3 разных символа</span>
      </div>
    </div>
  );

  return (
    <Message
      severity="error"
      className="justify-content-start w-full"
      text={content}
      style={{ padding: '12px 8px 12px 8px' }}
    />
  );
};
