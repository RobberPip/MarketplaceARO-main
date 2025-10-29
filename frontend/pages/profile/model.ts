import { createMutation, createQuery, update } from '@farfetched/core';
import { combine, createEffect, createEvent, createStore, sample } from 'effector';
import { reset, spread } from 'patronum';
import { FileUploadHandlerEvent } from 'primereact/fileupload';
import { z } from 'zod';
import { createApiEffectWithContract } from '~/shared/api/client';
import { components } from '~/shared/api/schema';
import { $viewer, applyAuthBarrier, chainAuthorized } from '~/shared/auth';
import { createForm, submit, updateErrors } from '~/shared/lib/form';
import { mapValidationErrorToForm } from '~/shared/lib/form-helpers';
import { controls, routes } from '~/shared/routing';
import { querySync } from 'atomic-router';
import { showToast } from '~/shared/notifications/model';
import { Dropdown } from '~/shared/ui/dropdown';

export const profileRoute = chainAuthorized(routes.profile.self);
type UpdateProfileIn = components['schemas']['UpdateProfileIn'];
type EmailDto = components['schemas']['EmailDto'];
type ChangeLoginDto = components['schemas']['ChangeLoginDto'];
type InfoRequestDto = components['schemas']['InfoRequest'];
type Role = components['schemas']['ProfileRoleOut'];

export const profileQuery = createQuery({
  ...createApiEffectWithContract('get', '/api/v1/profile'),
  mapData: (data) => data.result,
  initialData: {
    createdAt: '',
    updatedAt: '',
    avatar: {},
    userId: '-',
    userEmail: null,
    options: {
      roles: [],
      industries: [],
      companies: [],
    },
    accounts: {} as any, // TODO,
  },
});
applyAuthBarrier(profileQuery);
// TODO: how to use with barrier!?
// retry(profileQuery, { times: 5, delay: 1000);
export const updateProfileMutation = createMutation({
  ...createApiEffectWithContract('patch', '/api/v1/profile', {
    mapParams: (args: UpdateProfileIn) => {
      return { body: args };
    },
  }),
});
applyAuthBarrier(updateProfileMutation);

export const updateEmailMutation = createMutation({
  ...createApiEffectWithContract('post', '/auth/change-email', {
    mapParams: (args: components['schemas']['EmailDto']) => {
      return { body: args };
    },
  }),
});
applyAuthBarrier(updateEmailMutation);

export const updatePasswordMutation = createMutation({
  ...createApiEffectWithContract('post', '/api/v1/auth/info', {
    mapParams: (args: components['schemas']['InfoRequest']) => {
      return { body: args };
    },
  }),
});
applyAuthBarrier(updatePasswordMutation);

export const updateLoginMutation = createMutation({
  ...createApiEffectWithContract('post', '/auth/change-login', {
    mapParams: (args: components['schemas']['ChangeLoginDto']) => {
      return { body: args };
    },
  }),
});
applyAuthBarrier(updateLoginMutation);

export const avatarUploadMutation = createMutation({
  handler: async (event: FileUploadHandlerEvent) => {
    const file = event.files[0];

    const formData = new FormData();
    formData.append('imageFile', file, file.name);

    await fetch('/api/v1/profile/avatar', {
      method: 'POST',
      body: formData,
    });

    event.options.clear();
  },
});
applyAuthBarrier(avatarUploadMutation);

export const updateCompanyValue = createEvent<{ companyName: string }>();
export const updateOrCreateCompany = createEvent();
export const updateCheckedAgreements = createEvent<boolean>();

// TODO:
export const $email = $viewer.map((v) => v?.email ?? null);
export const $newEmail = $viewer.map((v) => v?.email ?? null);
export const $login = $viewer.map((v) => v?.login ?? null);
export const $firstName = $viewer.map((p) => p?.firstName ?? null);
export const $lastName = $viewer.map((p) => p?.lastName ?? null);

export const $roles = profileQuery.$data.map(({ options }) => groupRolesByCategory(options.roles));
export const $industries = profileQuery.$data.map(({ options }) => options.industries);

export const $companies = profileQuery.$data.map(({ options }) => options.companies);

export const $checkedAgreements = createStore<boolean>(false).on(updateCheckedAgreements, (_, v) => v);
export const $action = createStore<'' | 'confirmedEmail' | 'confirmedLogin' | null>(null);

// Update Company
export const $companyValue = createStore<string | null>(null)
  .on(updateCompanyValue, (_, { companyName }) => companyName)
  .on(profileQuery.$data, (_, { company }) => company?.name ?? null);

export const $companySuggest = combine(
  [$companyValue, $companies],
  ([value, companies]) =>
    companies.find((c) => (value ? c.name?.toLowerCase().startsWith(value.toLowerCase() ?? '') : null)) ?? null,
);

export const $companySuggestValue = combine([$companyValue, $companySuggest], ([value, suggest]) => {
  const suggesName = suggest?.name ?? '';

  return (value ?? '') + suggesName.slice(value?.length ?? 1 - 1);
});

/** Modal logic */
export const updateIsNextStep = createEvent<boolean>();
export const updateTimeLeft = createEvent<number>();
export const updateIsEmailSend = createEvent<boolean>();
export const openPassModal = createEvent();
export const closeModal = createEvent();

export const $isNextStep = createStore(false).on(updateIsNextStep, (_, v) => v);
export const $timeLeft = createStore(0).on(updateTimeLeft, (_, v) => v);
export const $isEmailSend = createStore(false).on(updateIsEmailSend, (_, v) => v);
export const $isPassModalOpen = createStore(false)
  .on(openPassModal, () => true)
  .on(closeModal, () => false);

sample({
  clock: [updateEmailMutation.finished.success, updateLoginMutation.finished.success],
  fn: () => ({
    timeLeft: 60,
    isNextStep: true,
    isEmailSend: true,
  }),
  target: spread({
    timeLeft: $timeLeft,
    isNextStep: $isNextStep,
    isEmailSend: $isEmailSend,
  }),
});

sample({
  clock: [updateProfileMutation.finished.success],
  fn: () => {
    return {
      severity: 'success',
      life: 1000,
      summary: 'Данные профиля успешно изменены',
    } as const;
  },
  target: showToast,
});

/** Change login Form */

const ChangeLoginDto = z.object({
  login: z.string().nonempty('Введите корректный логин'),
});

export const changeLoginForm = createForm({
  defaultValues: {
    login: '',
  },
  validators: {
    onSubmit: ChangeLoginDto,
    onBlur: ChangeLoginDto,
  },
});
submit(updateLoginMutation, {
  on: changeLoginForm,
});
updateErrors(changeLoginForm, {
  on: updateLoginMutation,
  fn: (args) => {
    const baseErrors = mapValidationErrorToForm(args);
    // Ранний выход
    if (!baseErrors) {
      return { errors: {} };
    }
    const { ...errors } = baseErrors.errors;
    errors.login = Object.entries(errors).map(
      ([message]) => `server: ${Array.isArray(message) ? message.join(', ') : message}`,
    );
    return { errors };
  },
});

/** Change email Form */

const EmailDto = z.object({
  email: z.string().email('Введите корректный адрес электронной почты'),
});

export const changeEmailForm = createForm({
  defaultValues: {
    email: '',
  },
  validators: {
    onSubmit: EmailDto,
    onBlur: EmailDto,
  },
});
submit(updateEmailMutation, {
  on: changeEmailForm,
});
updateErrors(changeEmailForm, {
  on: updateEmailMutation,
  fn: (args) => {
    const baseErrors = mapValidationErrorToForm(args);
    // Ранний выход
    if (!baseErrors) {
      return { errors: {} };
    }
    const { ...errors } = baseErrors.errors;
    errors.email = Object.entries(errors).map(
      ([message]) => `server: ${Array.isArray(message) ? message.join(', ') : message}`,
    );
    return { errors };
  },
});

/** Change password Form */
const PasswordOnlyDto = z.object({
  password: z
    .string()
    .min(8, { message: 'passwordtooshort' })
    .refine((val) => /[a-zA-Z]/.test(val), {
      message: 'passwordrequiresletter',
    })
    .refine((val) => /\d/.test(val), {
      message: 'passwordrequiresdigit',
    })
    .refine(
      (val) => {
        const uniqueChars = new Set(val);
        return uniqueChars.size >= 3;
      },
      {
        message: 'passwordrequiresuniquechars',
      },
    ),
});
const InfoRequestDto = z.object({
  oldPassword: z.string().nonempty(),
  newPassword: PasswordOnlyDto.shape.password,
});
export const changePasswordForm = createForm({
  defaultValues: {
    oldPassword: '',
    newPassword: '',
  },
  validators: {
    onSubmit: InfoRequestDto,
    onChange: (values) => {
      const errors = {
        fields: {} as Record<string, any>,
      };
      if (values.value.newPassword) {
        try {
          PasswordOnlyDto.parse({ password: values.value.newPassword });
        } catch (error) {
          if (error instanceof z.ZodError) {
            errors.fields.newPassword = error.errors.map((err) => err.message);
          }
        }
      }
      return errors;
    },
  },
});
submit(updatePasswordMutation, {
  on: changePasswordForm,
});
updateErrors(changePasswordForm, {
  on: updatePasswordMutation,
  fn: (args) => {
    const baseErrors = mapValidationErrorToForm(args);
    // Ранний выход
    if (!baseErrors) {
      return { errors: {} };
    }
    const { passwordMismatch, ...errors } = baseErrors.errors;

    if (passwordMismatch) {
      errors.oldPassword = Array.isArray(passwordMismatch) ? passwordMismatch : [passwordMismatch];
    }
    const newPasswordErrors = Object.entries(errors)
      .filter(([key]) => key !== 'oldPassword')
      .map(([key, message]) => `${key}: ${Array.isArray(message) ? message.join(', ') : message}`);

    errors.newPassword = newPasswordErrors;

    return { errors };
  },
});

sample({
  clock: updatePasswordMutation.finished.success,
  fn: () => {
    return {
      severity: 'success',
      life: 1000,
      summary: 'Ваш пароль успешно изменён',
    } as const;
  },
  target: [showToast, closeModal],
});
sample({
  clock: updatePasswordMutation.finished.success,
  source: changePasswordForm.$form,
  target: createEffect((form: any) => form.reset()),
});

/** Profile Form */

const UpdateProfileIn = z.object({
  firstName: z.string().min(2),
  lastName: z.string().min(2),
  description: z.string().optional().nullable(),
  companyId: z.string().uuid().optional().nullable(),
  industryId: z.string().uuid().optional().nullable(),
  roleId: z.string().uuid().optional().nullable(),
  companyName: z.string().optional().nullable(),
});
export const profileForm = createForm<Partial<UpdateProfileIn>>({
  defaultValues: profileQuery.$data.map((p) => ({
    firstName: p.firstName ?? '',
    lastName: p.lastName ?? '',
    description: p.description,
    companyId: p.company?.id,
    industryId: p.industry?.id,
    roleId: p.role?.id,
    companyName: null,
  })),
  validators: {
    onSubmit: UpdateProfileIn,
    onBlur: UpdateProfileIn,
  },
});

submit(updateProfileMutation, {
  on: profileForm,
});
updateErrors(profileForm, {
  on: updateProfileMutation,
  fn: mapValidationErrorToForm,
});

sample({
  clock: updateCompanyValue,
  source: { companyName: $companyValue, companySuggest: $companySuggest, form: profileForm.$form },
  filter: ({ companyName }) => (companyName?.length ?? 0) > 1,
  target: createEffect(({ companyName, companySuggest, form }: any) => {
    if (companySuggest) {
      form.setFieldValue('companyId', companySuggest.id);
      return;
    }
    form.setFieldValue('companyName', companyName!);
  }),
});

/** Profile load */

sample({
  clock: [routes.profile.self.updated, routes.profile.self.opened],
  target: profileQuery.start,
});

querySync({
  source: { action: $action },
  route: routes.profile.self,

  controls,
});

sample({
  clock: $action,
  filter: (action) => action !== null && ['confirmedEmail', 'confirmedLogin'].includes(action),
  fn: (action) => {
    let summary = '';
    switch (action) {
      case 'confirmedEmail':
        summary = 'Ваша почта успешно изменена';
        break;
      case 'confirmedLogin':
        summary = 'Ваш логин успешно изменен';
        break;
    }
    return {
      severity: 'success',
      life: 1000,
      summary,
    } as const;
  },
  target: showToast,
});

reset({
  clock: updateProfileMutation.finished.success,
  target: $action,
});
sample({
  clock: [routes.profile.self.opened, routes.profile.self.updated],
  fn: () => '' as const,
  target: $action,
});

/** Profile edit */

update(profileQuery, {
  on: avatarUploadMutation,
  by: {
    success: {
      source: profileQuery.$data,
      fn: (_, result) => {
        return { result, refetch: true };
      },
    },
    failure: {
      source: profileQuery.$data,
      fn: (_, result) => {
        return { result, refetch: true };
      },
    },
  },
});

update(profileQuery, {
  on: updateProfileMutation,
  by: {
    success: {
      source: profileQuery.$data,
      fn: ({ mutation }, result) => ({ result: { ...result, ...mutation.result } }),
    },
  },
});

// Dropdown roles
export function groupRolesByCategory(roles: Role[]): Array<{
  label: string;
  items: Role[];
}> {
  const grouped = roles.reduce<Record<string, Role[]>>((acc, role) => {
    const category = role.category || 'Без категории';
    if (!acc[category]) {
      acc[category] = [];
    }
    acc[category].push(role);
    return acc;
  }, {});

  return Object.entries(grouped).map(([category, items]) => ({
    label: category,
    items,
  }));
}
