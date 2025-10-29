export interface ProductAttributes {
  /** Применение */
  Activity: string;
  /** Тип работы */
  WorkType: string;
  /** Стадия жизненного цикла */
  StageType: string;
  /** Класс ПО*/
  ClassificationContent: string;
  /** Марка ПО*/
  ProjectMark: string;
  /** Работает с ЦИМ */
  WorkCIM: boolean;
}

export type ProductAttributeName = keyof ProductAttributes;
