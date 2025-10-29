import { RouteInstance } from 'atomic-router';
import { createEffect, createEvent, createStore, sample } from 'effector';
import { MenuItem } from 'primereact/menuitem';
import { routes } from '~/shared/routing';

export type NavMenuItem = MenuItem & { parent?: string; route: RouteInstance<any> };

export const navigate = createEvent<string>();

const menuItemsBase = [
  {
    id: 'profile',
    label: 'Профиль',
    icon: 'icon-user-circle',
    className: 'mt-4',
    route: routes.profile.self,
  },
  { id: 'account', label: 'Аккаунт', icon: 'icon-id', route: routes.profile.account, tag: 'new' },
  {
    id: 'products',
    label: 'Продукты',
    icon: 'icon-cube',
    route: routes.profile.products,
    disabled: false,
    tag: 'soon',
  },
  {
    id: 'subscriptions',
    label: 'Подписки',
    icon: 'icon-shopping-cart',
    route: routes.profile.subscriptions,
    disabled: true,
  },
  // { id: 'active-subscriptions', label: 'Активные подписки', parent: 'subscriptions', disabled: true },
  // { id: 'inactive-subscriptions', label: 'Неактивные подписки', parent: 'subscriptions', disabled: true },
  // { id: 'wishlist', label: 'Список желаемого', parent: 'subscriptions', disabled: true },

  // { id: 'active-products', label: 'Активные продукты', parent: 'products' },
  // { id: 'inactive-products', label: 'Неактивные продукты', parent: 'products' },
  // { id: 'subscription-plans', label: 'Планы подписок', parent: 'products' },

  { id: 'finances', label: 'Финансы', icon: 'icon-currency-rubel', route: routes.profile.finances, disabled: true },
  // { id: 'balance', label: 'Баланс', parent: 'finances', disabled: true },
  // { id: 'bank-details', label: 'Реквизиты', parent: 'finances', disabled: true },
  // { id: 'payments', label: 'Платежи', parent: 'finances', disabled: true },
  // { id: 'transaction-history', label: 'История операций', parent: 'finances', disabled: true },
  // { id: 'bank-details-duplicate', label: 'Реквизиты', parent: 'finances', disabled: true },

  { id: 'forum', label: 'Форум', icon: 'icon-message', route: routes.forum, disabled: true },
];
const $menuDict = createStore<Record<string, NavMenuItem>>(initmenuDict());
export const $menuItems = $menuDict.map((dict) => Object.values(dict));
export const $activeItemsIds = createStore<string[]>([]);

// Navigation
{
  sample({
    clock: navigate,
    source: $menuDict,
    fn: (menuDict, id) => {
      const items = [id];
      const parent = menuDict[id].parent;
      if (parent) {
        items.push(parent);
      }
      return items;
    },
    target: $activeItemsIds,
  });
  sample({
    clock: navigate,
    source: $menuDict,
    fn: (dict, id) => {
      return dict[id];
    },
    target: createEffect((item: NavMenuItem) => {
      item.route.open({});
      // Scroll to sub header
      if (item.parent) {
        setTimeout(() => {
          const el = document.getElementById(`${item.id!}-h`);
          if (el) {
            el.scrollIntoView({ block: 'start', behavior: 'smooth' });
          }
        }, 10);
      }
    }),
  });
}

function initmenuDict() {
  const menuDict: Record<string, NavMenuItem> = {};
  for (const item of menuItemsBase) {
    (menuDict as any)[item.id] = item;
  }
  for (const item of menuItemsBase) {
    if ('parent' in item && typeof item.parent === 'string' && typeof item.id === 'string') {
      (menuDict[item.id] as NavMenuItem).route = (menuDict[item.parent] as NavMenuItem).route;
    }
  }
  return menuDict;
}
