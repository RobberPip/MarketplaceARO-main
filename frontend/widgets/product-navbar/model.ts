import { createEvent, createStore, sample } from 'effector';
import { MenuItem } from 'primereact/menuitem';
import { GeneralInformation } from '../product-form/ui/general-info';
import { $activeView, $itemsProducts } from '../product-form/model';
// import { Step2 } from '../product-form/ui/step-2';

export type NavMenuItem = MenuItem & { parent?: string; view: React.ComponentType };

export const navigate = createEvent<string>();

const menuItemsBase = [
  {
    id: 'general-info',
    label: 'Общая информация',
    icon: 'icon-list',
    className: 'mt-4',
    view: GeneralInformation,
  },
  {
    id: 'classification',
    label: 'Классификация',
    icon: 'icon-category-plus',
    disabled: true,
  },
  {
    id: 'technical-support',
    label: 'Техническая поддержка',
    icon: 'icon-message-circle-question',
    disabled: true,
  },
  {
    id: 'distribution',
    label: 'Распространение',
    icon: 'icon-file-export',
    disabled: true,
  },
  {
    id: 'technical-specifications',
    label: 'Технические характеристики',
    icon: 'icon-settings',
    disabled: true,
  },
  {
    id: 'subscription-plan',
    label: 'План подписок',
    icon: 'icon-shopping-cart',
    disabled: true,
  },
];
const $menuDict = createStore<Record<string, NavMenuItem>>(initmenuDict());
export const $menuItems = $menuDict.map((dict) => Object.values(dict));
export const $activeItemsIds = createStore<string[]>([menuItemsBase[0].id]);

// Navigation
{
  sample({
    clock: navigate,
    source: $menuDict,
    fn: (menuDict, id) => {
      const items = [id];
      const parent = menuDict[id]?.parent;
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
    fn: (menuDict, id) => {
      const label = menuDict[id]?.label || '';
      return [label];
    },
    target: $itemsProducts,
  });

  sample({
    clock: navigate,
    source: { menuDict: $menuDict, $menuItems },
    fn: ({ menuDict }, id) => {
      const item = menuDict[id];
      if (item?.view) {
        return item.view;
      }
      return null;
    },
    target: $activeView,
  });
}

function initmenuDict() {
  const menuDict: Record<string, NavMenuItem> = {};
  for (const item of menuItemsBase) {
    (menuDict as any)[item.id] = item;
  }
  for (const item of menuItemsBase) {
    if ('parent' in item && typeof item.parent === 'string' && typeof item.id === 'string') {
    }
  }
  return menuDict;
}
