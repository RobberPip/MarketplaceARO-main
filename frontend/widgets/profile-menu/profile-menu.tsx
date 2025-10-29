import { useUnit } from 'effector-react';
import { Menu } from 'primereact/menu';
import React, { useEffect, useRef } from 'react';
import { $email, $firstName, $lastName } from '~/pages/profile/model.ts';
import { $authorized, $viewer, logout } from '~/shared/auth';
import { $isWidget, $mainMenuItems, $menuItems, $newAccountItem } from './model.ts';
import { Link } from 'atomic-router-react';
import { routes } from '~/shared/routing.ts';
import { Avatar } from 'primereact/avatar';
import clsx from 'clsx';
import { $activeAccountAvatar, $activeAccountId, changeAccountMutation } from '~/pages/profile-account/model.ts';
import { components } from '~/shared/api/schema.js';

export const ProfileMenu: React.FC = () => {
  const [firstName, lastName, userEmail, authorized, isWidget] = useUnit([
    $firstName,
    $lastName,
    $email,
    $authorized,
    $isWidget,
  ]);

  const viewer = useUnit($viewer);
  const userName = `${lastName} ${firstName}`;
  const menuItems = useUnit($mainMenuItems);
  const newAccountItem = useUnit($newAccountItem);
  const activeAccountId = useUnit($activeAccountId);
  const menuRef = useRef<Menu>(null);
  const privateAccounts =
    viewer?.accounts?.filter((acc) => acc.type === 'SelfEmployed' || acc.type === 'IndividualEntrepreneur') ?? [];
  const otherAccounts =
    viewer?.accounts?.filter((acc) => acc.type !== 'SelfEmployed' && acc.type !== 'IndividualEntrepreneur') ?? [];
  const activeAccountObj = viewer?.accounts?.find((acc) => acc.id === activeAccountId);
  const userRole = activeAccountObj?.userRole ?? null;
  const isOwnerOrManager = userRole === 'Owner' || userRole === 'Manager';
  const accountAvatarUrl = activeAccountObj?.brand?.avatar?.url ?? null;
  const profileAvatarUrl = viewer?.avatar ?? null;
  const accountInitials = (activeAccountObj?.brand?.publicName ?? '').slice(0, 2).toUpperCase();
  const viewerInitials =
    viewer?.firstName && viewer?.lastName ? viewer.firstName[0].toUpperCase() + viewer.lastName[0].toUpperCase() : null;
  const avatarToShow = isOwnerOrManager ? accountAvatarUrl || null : profileAvatarUrl || null;

  useEffect(() => {
    const onScroll = (e: Event) => menuRef.current?.hide(e as any);
    document.addEventListener('scroll', onScroll);
    return () => document.removeEventListener('scroll', onScroll);
  }, []);

  const renderAccountItem = (account: components['schemas']['AccountsDto']) => {
    const isActive = activeAccountId === account.id;

    return (
      <div
        className={clsx('p-menuitem-content cursor-pointer flex items-center py-2', {
          'surface-300 font-semibold': isActive,
        })}
        key={account.id}
        onClick={() => {
          if (typeof account.id === 'string') {
            changeAccountMutation.start({ accountId: account.id });
          }
        }}
      >
        <div className="text-sm flex flex-column mx-3 flex-grow-1">
          <span className="font-medium mb-1 white-space-nowrap w-12rem text-overflow-ellipsis overflow-hidden">
            {account.brand.publicName}
          </span>
          <span className="text-xs text-color-secondary">{account.shortName}</span>
        </div>
        {isActive && (
          <div className="flex align-items-center mr-2">
            <span className="icon-3xl icon-check" />
          </div>
        )}
      </div>
    );
  };

  return (
    <>
      {authorized ? (
        <Avatar
          style={{ marginLeft: '29px', userSelect: 'none' }}
          className={clsx('mr-3', { 'border-1 bg-white': !avatarToShow })}
          shape="circle"
          image={avatarToShow ?? undefined}
          onClick={(e) => {
            menuRef.current?.toggle(e);

            if (isWidget) {
              setTimeout(() => {
                const el = menuRef.current?.getElement();
                if (!el) return;
                const prev = Number(el.style.top.replace('px', ''));
                el.style.top = `${prev - 60}px`;
              }, 0);
            }
          }}
        >
          {!avatarToShow && (isOwnerOrManager ? accountInitials : viewerInitials)}
        </Avatar>
      ) : isWidget ? (
        <a
          style={{ width: '60px', height: '38px', display: 'flex', alignItems: 'center' }}
          className="nav-link px-3 py-1"
          href="/app/login"
        >
          Войти
        </a>
      ) : (
        <Link className="p-menuitem-link" to={routes.signin}>
          <span className="p-menuitem-text cursor-pointer text-500 hover:text-900" data-pc-section="label">
            Войти
          </span>
        </Link>
      )}

      <Menu
        className="border-round-xl border-1 w-16rem pb-0"
        model={[
          {
            template: () => (
              <div className="flex align-items-center">
                <Avatar
                  className="border-1 flex-shrink-0 ml-3 my-2 bg-white select-none"
                  shape="circle"
                  image={avatarToShow ?? undefined}
                >
                  {!avatarToShow && (isOwnerOrManager ? accountInitials : viewerInitials)}
                </Avatar>
                <div className="p-menuitem-header ml-2">
                  <h1 className="text-base font-semibold white-space-nowrap w-12rem text-overflow-ellipsis overflow-hidden">
                    {userName || 'Пользователь'}
                  </h1>
                  <span className="text-500  white-space-nowrap w-12rem text-overflow-ellipsis overflow-hidden">
                    {activeAccountObj?.brand.publicName}
                  </span>
                </div>
              </div>
            ),
          },
          { separator: true, className: 'mt-0' },
          ...menuItems.map((item) => ({
            ...item,
            className: 'font-medium',
          })),
          ...(privateAccounts.length > 0
            ? [
                { separator: true, className: 'mb-0' },
                ...privateAccounts.map((account) => ({
                  template: () => renderAccountItem(account),
                })),
              ]
            : []),
          ...(otherAccounts.length > 0
            ? [
                { separator: true, className: 'mb-0' },
                ...otherAccounts.map((account) => ({
                  template: () => renderAccountItem(account),
                })),
              ]
            : []),
          { separator: true, className: 'mt-0' },
          ...(newAccountItem ? [newAccountItem] : []),
          { separator: true },
          {
            label: 'Выйти',
            template: (item, options) => (
              <div
                className="p-menuitem-exit flex items-center gap-2"
                onClick={(e) => {
                  options.onClick?.(e);
                  logout();
                }}
              >
                <span className="icon-2xl icon-logout mb-1 p-menuitem-red" />
                {item.label}
              </div>
            ),
          },
        ]}
        popup
        ref={menuRef}
      />
    </>
  );
};
