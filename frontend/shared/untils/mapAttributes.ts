import { components } from '../api/schema';

export const mapAttributes = <T extends string>(attributes: components['schemas']['GetAttributesDataOut'][]) => {
  return attributes.reduce<Record<T, string | number | boolean>>(
    (acc, attr) => {
      if (!attr.attributeName || attr.value === undefined || attr.value === null) return acc;

      let value: string | number | boolean;

      switch (attr.dataType) {
        case 'decimal':
        case 'integer':
          value = Number(attr.value);
          break;
        case 'boolean':
          value = Boolean(attr.value);
          break;
        default:
          value = String(attr.value);
      }

      acc[attr.attributeName as T] = value;
      return acc;
    },
    {} as Record<T, string | number | boolean>,
  );
};
