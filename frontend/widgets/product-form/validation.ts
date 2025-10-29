import { z } from 'zod';

export const ProductIn = z.object({
  name: z.string().min(3, 'Наименование слишком короткое'),
  description: z.string().min(10, 'Описание слишком короткое'),
  website: z.string().url('Введите корректный URL'),
  typeId: z.string().uuid('Введите корректный GUID'),
  images: z.array(z.any()).optional(),
  rightholders: z.array(
    z.object({
      id: z.string(),
      shortName: z.string().optional(),
    }),
  ),
  productData: z.object({
    VideoUrl: z.string(),
    // applicationType: z.enum(['General', 'Plugin']),
    // mainProduct: z.array(z.string()).optional(),
    // icon: z.any(),
    // lifecycleStage: z.array(z.string()),
    // businessProcess: z.string(),
    // class: z.string(),
    // impactObject: z.array(z.string()),
    // projectMark: z.array(z.string()),
    // worksWithCIM: z.boolean(),
    // inNOTIM: z.boolean().default(true),
    // inRegistry: z.boolean(),
    // registryCode: z.string().optional(),
    // registryName: z.string().optional(),
    // registryLink: z.string().optional(),
    // media: z.array(z.any()).max(10, 'Максимум 10 фото или 3 видео'),
    // language: z.array(z.string()).min(1, 'Хотя бы один вариант языка обязателен'),
    // os: z.array(z.string()).min(1, 'Хотя бы один вариант ОС обязателен'),
    // releaseDate: z.date(),
    // updateDate: z.date(),
    // versionInfo: z.string(),
    // customerSupport: z.string(),
    // subscriptions: z.array(SubscriptionIn).min(1, 'Хотя бы один вариант подписки обязателен'),
  }),
});

export const productGeneralSchema = z.object({
  name: z.string().min(3, 'Наименование слишком короткое'),
  file: z.any().optional(),
});

export const stepOneSchema = z.object({
  applicationType: z.enum(['General', 'Plugin']),
  mainProduct: z.array(z.string()).optional(),
  website: z.string().url('Введите корректный URL'),
  description: z.string().min(10, 'Описание слишком короткое'),
  icon: z.any(),
  lifecycleStage: z.array(z.string()),
  businessProcess: z.string(),
  class: z.string(),
  impactObject: z.array(z.string()),
  projectMark: z.array(z.string()),
  worksWithCIM: z.boolean(),
  inNOTIM: z.boolean().default(true),
  inRegistry: z.boolean(),
  registryCode: z.string().optional(),
  registryName: z.string().optional(),
  registryLink: z.string().optional(),
});

export const stepTwoSchema = z.object({
  media: z.array(z.any()).max(10, 'Максимум 10 фото или 3 видео'),
  language: z.array(z.string()).min(1, 'Хотя бы один вариант языка обязателен'),
  os: z.array(z.string()).min(1, 'Хотя бы один вариант ОС обязателен'),
  releaseDate: z.date(),
  updateDate: z.date(),
  versionInfo: z.string(),
  customerSupport: z.string(),
});

export const stepThreeSchema = z.object({
  subscriptions: z
    .array(
      z.object({
        name: z.string().min(3, 'Наименование слишком короткое'),
        type: z.string(),
        description: z.string().min(10, 'Описание слишком короткое'),
        included: z.array(z.string()),
      }),
    )
    .min(1, 'Хотя бы один вариант подписки обязателен'),
});

export const productValidationSchema = productGeneralSchema
  .merge(stepOneSchema)
  .merge(stepTwoSchema)
  .merge(stepThreeSchema);
