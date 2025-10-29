import { Button } from 'primereact/button';
import { PrivateAccount, NewPrivateAccount } from './ui/private-account';
import { NewPublicAccount, PublicAccount, PublicAccountForm } from './ui/public-account';
import { useRef } from 'react';
import { Menu } from 'primereact/menu';
import { useUnit } from 'effector-react';
import {
  $isNew,
  $resolvedType,
  accountQuery,
  changeAccountMutation,
  setIsSubmitted,
  setSelectedAccount,
  setSelectedFile,
  updateBrandForm,
} from './model';
import { components } from '~/shared/api/schema';
import { Card } from 'primereact/card';
import { Avatar } from 'primereact/avatar';
import { $viewer } from '~/shared/auth';

export function AccountView() {
  const isNew = useUnit($isNew);
  const accounts = useUnit($viewer.map((q) => q?.accounts));
  const resolvedType = useUnit($resolvedType);

  const getContent = () => {
    const views: Record<string, React.ReactNode> = {
      public: <PublicAccount />,
      private: <PrivateAccount />,
    };

    const formViews: Record<string, React.ReactNode> = {
      publicForm: <NewPublicAccount />,
      privateForm: <NewPrivateAccount />,
    };

    if (isNew) {
      if (resolvedType && formViews[resolvedType]) {
        return formViews[resolvedType];
      }

      return (accounts?.length ?? 0) > 0 ? <ExistingAccounts /> : <EmptyAccounts />;
    }

    return resolvedType && views[resolvedType] ? views[resolvedType] : <EmptyAccounts />;
  };

  return <main className="mb-4">{getContent()}</main>;
}

export const itemTemplate = (item: components['schemas']['GetCompanyOut']) => (
  <div>
    <div>
      <strong>{item.inn}</strong>
    </div>
    <div className="text-xs">{item.fullName}</div>
  </div>
);

const ExistingAccounts = () => {
  const viewer = useUnit($viewer);
  const activeAccount = useUnit(accountQuery.$data);
  const selectAccount = useUnit(setSelectedAccount);
  const menu = useRef<Menu>(null);
  const { extendedForm: formBrand } = useUnit(updateBrandForm);
  const resetSelectedFile = useUnit(setSelectedFile);
  const updateIsSubmitted = useUnit(setIsSubmitted);
  const resetFormBrand = () => {
    formBrand.reset({
      publicName: '',
      description: '',
      mainActivityIds: [],
      website: '',
      email: '',
      phoneNumber: '',
      publicListIds: [],
    });
    resetSelectedFile(null);
  };
  const hasIEorSE = (viewer?.accounts ?? []).some(
    (account) => account.type === 'IndividualEntrepreneur' || account.type === 'SelfEmployed',
  );

  const menuModel = [
    ...(!hasIEorSE
      ? [
          {
            label: 'Приватный',
            command: () => {
              resetFormBrand();
              selectAccount('privateForm');
              updateIsSubmitted(false);
            },
          },
        ]
      : []),
    {
      label: 'Публичный',
      command: () => {
        resetFormBrand();
        selectAccount('publicForm');
        updateIsSubmitted(false);
      },
    },
  ];
  return (
    <>
      <div className="flex justify-content-between">
        <h1 className="m-0 text-2xl font-medium">Аккаунт</h1>
        <Button className="w-10rem" label="Добавить аккаунт" onClick={(event) => menu.current?.toggle(event)} />
        <Menu className="w-10rem text-color-secondary border-round-xl border-1" model={menuModel} popup ref={menu} />
      </div>

      <div className="flex flex-column justify-content-center align-items-center" style={{ marginTop: '110px' }}>
        <h2 className="m-0 text-lg font-medium mb-5">Существующие аккаунты</h2>
        <div className="w-29rem" style={{ maxHeight: '246x', overflowY: 'auto' }}>
          {(viewer?.accounts ?? []).map((account) => {
            const isActive = account.id === activeAccount.id;

            return (
              <Card
                onClick={() => changeAccountMutation.start({ accountId: account.id })}
                key={account.id}
                className={`mb-2 cursor-pointer transition-colors ${isActive ? 'surface-300' : 'hover:bg-gray-50'}`}
                pt={{
                  body: { className: 'py-2 p-0' },
                  content: { className: 'p-0' },
                }}
              >
                <div className="flex justify-content-between align-items-center px-3">
                  <div className="flex align-items-center overflow-hidden">
                    <Avatar
                      image={account.brand.avatar?.url || undefined}
                      label={
                        !account.brand.avatar?.url
                          ? (account.brand.publicName ?? '').slice(0, 2).toUpperCase()
                          : undefined
                      }
                      shape="circle"
                      className="mr-2 border-circle border-1 flex-shrink-0 bg-white"
                      style={{ width: '32px', height: '32px' }}
                    />

                    <div className="flex flex-column min-w-0">
                      <h2 className="text-base font-semibold m-0">{account.brand.publicName}</h2>
                      <span className="text-color-secondary white-space-nowrap overflow-hidden text-overflow-ellipsis">
                        {account.shortName}
                      </span>
                    </div>
                  </div>
                  {isActive && <span className="icon-3xl icon-check" />}
                </div>
              </Card>
            );
          })}
        </div>
      </div>
    </>
  );
};
const EmptyAccounts = () => {
  const menu = useRef<Menu>(null);
  const selectAccount = useUnit(setSelectedAccount);
  const { extendedForm: formBrand } = useUnit(updateBrandForm);
  const resetSelectedFile = useUnit(setSelectedFile);
  const updateIsSubmitted = useUnit(setIsSubmitted);
  const resetFormBrand = () => {
    formBrand.reset({
      publicName: '',
      description: '',
      mainActivityIds: [],
      website: '',
      email: '',
      phoneNumber: '',
      publicListIds: [],
    });
    resetSelectedFile(null);
  };
  return (
    <>
      <h1 className="m-0 text-4xl mb-4 w-max">Аккаунт</h1>
      <div className="flex flex-column justify-content-center align-items-center" style={{ marginTop: '190px' }}>
        <h2 className="m-0 text-2xl font-medium mb-4">Нет аккаунтов</h2>
        <span className="mb-5 text-color-secondary text-center text-sm">
          Добавьте аккаунт, чтобы в полном объёме использовать
          <br />
          возможности IT market, в том числе чтобы приобретать и
          <br />
          распространять цифровые продукты
        </span>
        <Button className="w-10rem" label="Добавить аккаунт" onClick={(event) => menu.current?.toggle(event)} />
        <Menu
          className="w-10rem text-color-secondary border-round-xl border-1"
          model={[
            {
              label: 'Приватный',
              command: () => {
                resetFormBrand();
                selectAccount('privateForm');
                updateIsSubmitted(false);
              },
            },
            {
              label: 'Публичный',
              command: () => {
                resetFormBrand();
                selectAccount('publicForm');
                updateIsSubmitted(false);
              },
            },
          ]}
          popup
          ref={menu}
        />
      </div>
    </>
  );
};
export function StatusIcon({ status }: { status: 'Pending' | 'Approved' | 'Blocked' | undefined }) {
  let iconClass = '';

  if (status === 'Pending') {
    iconClass = 'icon-dots-circle-horizontal';
  } else if (status === 'Approved') {
    iconClass = 'icon-circle-check bg-blue-700';
  } else if (status === 'Blocked') {
    iconClass = 'icon-alert-circle bg-red-700';
  }

  if (!iconClass) {
    return null;
  }
  return <span className={`icon-2xl ${iconClass} ml-2`} />;
}
