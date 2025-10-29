import { useUnit } from 'effector-react';
import { Privacy } from '~/pages/legal-docs/ui/privacy.tsx';
import { $docName, docNameChanged } from './model';
import { Terms } from './ui/terms';
import { DevelopersAgreement } from './ui/developers-agreement';
import { PersonalData } from './ui/personal-data';
import { TermsOfUse } from './ui/terms-use';

const links = [
  { label: 'Согласие на обработку персональных данных', value: 'personal-data' },
  { label: 'Пользовательское соглашение', value: 'terms' },
  { label: 'Политика конфиденциальности', value: 'privacy' },
  { label: 'Соглашение о распространении программных продуктов', value: 'user-agreement' },
  { label: 'Условия использования', value: 'terms-use' },
];

export const DocsPage = () => {
  const doc = useUnit($docName);

  switch (doc) {
    case 'user-agreement':
      return <DevelopersAgreement />;
    case 'privacy':
      return <Privacy />;
    case 'terms':
      return <Terms />;
    case 'personal-data':
      return <PersonalData />;
    case 'terms-use':
      return <TermsOfUse />;
    default:
      return (
        <section>
          <h1 className="text-4xl font-medium mt-5">Правовая документация IT market</h1>

          <ul className="list-none p-0 text-sm">
            {links.map(({ label, value }) => (
              <li className="mb-3" key={value}>
                <a
                  href="#"
                  onClick={(e) => {
                    e.preventDefault();
                    docNameChanged(
                      value as '' | 'user-agreement' | 'personal-data' | 'privacy' | 'terms' | 'terms-use',
                    );
                  }}
                >
                  {label}
                </a>
              </li>
            ))}
          </ul>
        </section>
      );
  }
};
