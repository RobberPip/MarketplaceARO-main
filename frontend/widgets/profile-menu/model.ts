import { createStore } from 'effector';
import { not } from 'patronum';
import { MenuItem } from 'primereact/menuitem';
import { router, routes } from '~/shared/routing.ts';

export type NavMenuItem = MenuItem;

const $routerInitialized = createStore(false).on(router.initialized, () => true);
export const $isWidget = not($routerInitialized);
export const $menuItems = router.$activeRoutes.map((activeRoutes) => {
  const hasRouting = activeRoutes.length > 0;
  const profileItem: MenuItem = {
    id: 'profile',
    label: 'Профиль',
    icon: 'icon-2xl icon-user-circle',
  };
  const accountItem: MenuItem = { id: 'account', label: 'Аккаунт', icon: 'icon-2xl icon-id' };
  const productItem: MenuItem = { id: 'products', label: 'Продукты', icon: 'icon-2xl icon-cube' };
  const newAccountItem: MenuItem = {
    id: 'newAccount',
    label: 'Добавить аккаунт',
    icon: 'icon-2xl icon-plus',
    className: 'font-medium',
  };
  if (hasRouting) {
    profileItem.command = () => routes.profile.self.open();
    accountItem.command = () => routes.profile.account.open();
    productItem.command = () => routes.profile.products.open();
    newAccountItem.command = () =>
      routes.profile.account.navigate({
        query: { new: 'true' },
        params: {},
      });
  } else {
    // TODO
    profileItem.url = '/app/profile';
    accountItem.url = '/app/profile/account';
    newAccountItem.url = '/app/profile/account?new=true';
    productItem.url = '/app/profile/products';
  }

  return [
    profileItem,
    accountItem,
    productItem,
    newAccountItem,

    {
      id: 'subscriptions',
      label: 'Подписки',
      icon: 'icon-2xl icon-shopping-cart',
      disabled: true,
    },
    { id: 'finances', label: 'Финансы', icon: 'icon-2xl icon-currency-rubel', disabled: true },
  ] satisfies MenuItem[];
});

export const $mainMenuItems = $menuItems.map((items) => items.filter((item) => item.id !== 'newAccount'));
export const $newAccountItem = $menuItems.map((items) => items.find((item) => item.id === 'newAccount') ?? null);

export const $menuDict = $menuItems.map((menuItems) => {
  const menuDict: Record<string, NavMenuItem> = {};

  for (const item of menuItems) {
    if (item.id) {
      menuDict[item.id] = { ...item };
    }
  }

  return menuDict;
});
