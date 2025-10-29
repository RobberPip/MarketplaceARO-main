import { createMutation, createQuery, exponentialDelay, isHttpErrorCode, retry } from '@farfetched/core';
import { combine, createEffect, createEvent, createStore, merge, sample } from 'effector';
import { ApiError, isApiError } from 'openapi-ff';
import { debug, every, reset, spread } from 'patronum';
import { createApiEffectWithContract } from '~/shared/api/client';
import { components } from '~/shared/api/schema';
import { $next, authorized, logout, signInMutation } from '~/shared/auth';
import { showToast } from '~/shared/notifications';
import { controls, routes } from '~/shared/routing';
import { groupRolesByCategory } from '../profile/model';
import { querySync } from 'atomic-router';
import { z } from 'zod';
import { createForm, submit } from '~/shared/lib/form';
import { FormApi } from '@tanstack/react-form';

type LoginDto = components['schemas']['LoginDto'];
type RegistrationDto = components['schemas']['RegistrationDto'];
type ResetPassDto = components['schemas']['ResetPasswordRequest'];
type ForgotPassDto = components['schemas']['ForgotPasswordRequest'];
type Role = components['schemas']['ProfileRoleOut'];
export const registrationMutation = createMutation({
  ...createApiEffectWithContract('post', '/auth/register', {
    mapParams(args: components['schemas']['RegistrationDto']) {
      return { body: args };
    },
  }),
});

export const sendConfirmedEmailMutation = createMutation({
  ...createApiEffectWithContract('post', '/auth/send-confirm-email', {
    mapParams(args: components['schemas']['SendConfirmEmailDto']) {
      return { body: args };
    },
  }),
});

export const profileOptionsQuery = createQuery({
  ...createApiEffectWithContract('get', '/api/v1/profile/options'),
  mapData: (data) => data.result,
  initialData: {
    companies: [],
    roles: [],
    industries: [],
  },
});
export const updateProfileMutation = createMutation({
  ...createApiEffectWithContract('patch', '/api/v1/profile', {
    mapParams: (args: components['schemas']['UpdateProfileIn']) => {
      return { body: args };
    },
  }),
});
retry(updateProfileMutation, { times: 3, delay: exponentialDelay(1000) });

export const forgotPasswordMutation = createMutation({
  ...createApiEffectWithContract('post', '/api/v1/auth/forgot-password', {
    mapParams: (args: components['schemas']['ForgotPasswordRequest']) => {
      return { body: args };
    },
  }),
});

export const resetPasswordMutation = createMutation({
  ...createApiEffectWithContract('post', '/api/v1/auth/reset-password', {
    mapParams: (args: components['schemas']['ResetPasswordRequest']) => {
      return { body: args };
    },
  }),
});

export const signupRoute = routes.signup;
export const loginRoute = routes.signin;
export const resetpassRoute = routes.resetpass;

export const sendConfirmedEmail = createEvent();
export const submitProfile = createEvent();

export const updateIndustry = createEvent<string>();

export const updateRole = createEvent<string>();

export const updateCompany = createEvent<components['schemas']['ProfileCompanyOut']>();
export const updateCompanyValue = createEvent<{ companyName: string }>();
export const updateOrCreateCompany = createEvent();

export const $loginStep = createStore<'registration' | 'info' | 'reset' | null>(null);
export const $step = createStore<'reset' | null>(null);

export const $emailSent = createStore(false);

export const $formError = createStore<string | null>(null);
export const $emailError = createStore<string | null>(null, { skipVoid: false });
export const $passwordError = createStore<string | null>(null, { skipVoid: false });

export const $companies = profileOptionsQuery.$data.map(({ companies }) => companies);
export const $roles = profileOptionsQuery.$data.map((data) => groupRolesByCategory(data.roles ?? []));
export const $industries = profileOptionsQuery.$data.map((data) => data.industries ?? []);

export const $selectedRole = createStore<string | null>(null).on(updateRole, (_, v) => v);
export const $selectedCompany = createStore<components['schemas']['ProfileCompanyOut'] | null>(null);
export const $selectedIndustry = createStore<string | null>(null).on(updateIndustry, (_, v) => v);

// Company Selection
export const $companyValue = createStore<string | null>(null)
  .on(updateCompanyValue, (_, { companyName }) => companyName)
  .on($selectedCompany, (_, company) => company?.name);
export const $companySuggest = combine([$companyValue, $companies], ([value, companies]) => {
  return companies.find((c) => (value ? c.name?.toLowerCase().startsWith(value.toLowerCase() ?? '') : null)) ?? null;
});
export const $companySuggestValue = combine([$companyValue, $companySuggest], ([value, suggest]) => {
  const suggesName = suggest?.name ?? '';

  return (value ?? '') + suggesName.slice(value?.length ?? 1 - 1);
});

export const $isAnySelected = combine([$selectedRole, $selectedIndustry, $selectedCompany], (vals) =>
  vals.some((v) => v !== null),
);

sample({
  clock: updateCompanyValue,
  fn: ({ companyName: name }) => ({ id: crypto.randomUUID(), name, status: 'Init' }) as const,
  target: $selectedCompany,
});
sample({
  clock: updateOrCreateCompany,
  source: { companyName: $companyValue, companySuggest: $companySuggest },
  filter: ({ companyName }) => (companyName?.length ?? 0) > 1,
  fn: ({ companyName, companySuggest }) => {
    if (companySuggest) {
      return companySuggest;
    }
    return { id: crypto.randomUUID(), name: companyName!, status: 'Init' } as const;
  },
  target: $selectedCompany,
});

// Resets
{
  reset({
    clock: [loginRoute.opened, signupRoute.opened, resetpassRoute.opened],
    target: [
      $emailSent,
      $loginStep,
      $companyValue,
      $selectedIndustry,
      $selectedRole,
      $passwordError,
      $emailError,
      $formError,
    ],
  });
}

// Errors
// TODO:
// - add error response to API spec!
// - do not use 404
{
  const unexpectedError = sample({
    clock: [registrationMutation.finished.failure, signInMutation.finished.failure],
    filter: (err) => !isApiError(err) || isHttpErrorCode(500)(err),
  });

  const validationError = sample({
    clock: [
      registrationMutation.finished.failure,
      signInMutation.finished.failure,
      resetPasswordMutation.finished.failure,
    ],
    filter: (err) => isApiError(err) && err.error.status === 400,
    fn: ({ error }) => error as ApiError<400, any>,
  });

  const authError = sample({
    clock: [registrationMutation.finished.failure, signInMutation.finished.failure],
    filter: (err) => isApiError(err) && err.error.status === 401,
    fn: ({ error }) => error as ApiError<401, any>,
  });

  sample({
    clock: validationError.map(({ response }) => response),
    fn: (res) => ({
      email:
        Object.keys(res.errors)
          .filter((key) => key.includes('Email'))
          .join(', ') || null,
      password:
        Object.keys(res.errors)
          .filter((key) => key.includes('Password'))
          .join(', ') || null,
      form: Object.keys(res.errors).join(', ') || null,
    }),
    target: spread({
      email: $emailError,
      password: $passwordError,
      form: $formError,
    }),
  });
  sample({
    clock: authError,
    fn: () => 'Неверный логин или пароль',
    target: $formError,
  });

  sample({
    clock: unexpectedError,
    fn: () => {
      return {
        severity: 'error',
        life: 10000,
        summary: 'Неизвестная Ошибка',
      } as const;
    },
    target: showToast,
  });

  sample({
    clock: resetPasswordMutation.finished.failure,
    filter: routes.resetpass.$isOpened,
    fn: () => {
      return {
        severity: 'error',
        life: 10000,
        summary: 'Ссылка устарела',
        detail: 'Ссылка, отправленная на почту, устарела.\nПопробуйте заново восстановить пароль',
        contentStyle: { whiteSpace: 'pre-line' },
      } as const;
    },
    target: showToast,
  });
}

// Registration
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

const RegistrationDto = z.object({
  email: z.string().email('Введите корректный email'),
  password: PasswordOnlyDto.shape.password,
  checkedAgreements: z.literal(true),
  checkedRules: z.literal(true),
  data: z.object({
    firstName: z.string().nonempty(),
    lastName: z.string().nonempty(),
    invitationId: z.string(),
    token: z.string(),
  }),
});

export const registrationForm = createForm({
  defaultValues: {
    email: '',
    password: '',
    checkedAgreements: false,
    checkedRules: false,
    data: {
      firstName: '',
      lastName: '',
      invitationId: '',
      token: '',
    },
  },
  validators: {
    onSubmit: RegistrationDto,
    onChange: (values) => {
      const errors = {
        fields: {} as Record<string, any>,
      };
      if (values.value.password) {
        try {
          PasswordOnlyDto.parse({ password: values.value.password });
        } catch (error) {
          if (error instanceof z.ZodError) {
            errors.fields.password = error.errors.map((err) => err.message);
          }
        }
      }
      return errors;
    },
  },
});

submit(registrationMutation, {
  on: registrationForm,
});

sample({
  clock: [signupRoute.opened],
  target: profileOptionsQuery.start,
});

sample({
  clock: sendConfirmedEmail,
  source: registrationForm.$values.map((q) => q?.email),
  fn: (email) =>
    ({
      email,
    }) as components['schemas']['SendConfirmEmailDto'],
  target: sendConfirmedEmailMutation.start,
});
const registrationSuccessed = registrationMutation.finished.success;
$loginStep.on(registrationSuccessed, () => 'info');

sample({
  clock: submitProfile,
  source: {
    roleId: $selectedRole,
    company: $selectedCompany,
    industryId: $selectedIndustry,
  },
  fn: ({ roleId, company, industryId }) => {
    const companyId = company && company.status !== 'Init' ? company.id : null;
    const companyName = company && company.status === 'Init' ? company.name : null;
    return { roleId, companyId, companyName, industryId } as components['schemas']['UpdateProfileIn'];
  },
  target: updateProfileMutation.start,
});
// TODO: handle errors
$emailSent.on(updateProfileMutation.finished.finally, () => true);

// Login
const LoginDto = z.object({
  email: z.string().email('Введите корректный email'),
  password: z.string().nonempty(),
  rememberMe: z.boolean(),
  data: z.object({
    invitationId: z.string(),
    token: z.string(),
  }),
});
export const loginForm = createForm({
  defaultValues: {
    password: '',
    email: '',
    rememberMe: false,
    data: {
      invitationId: '',
      token: '',
    },
  },
  validators: {
    onSubmit: LoginDto,
    onChange: LoginDto,
  },
});
const setEmailFx = createEffect((email: string) => {
  const form = loginForm.$extendedForm.getState();
  form?.setFieldValue('email', email);
});

sample({
  clock: registrationForm.$values,
  fn: (values) => values?.email ?? '',
  filter: (email) => typeof email === 'string' && email !== '',
  target: setEmailFx,
});

submit(signInMutation, {
  on: loginForm,
});
const openRoute = createEffect((args: { path: string }) => {
  window.open(args.path, '_self');
});
// redirect after success login
sample({
  clock: authorized,
  filter: loginRoute.$isOpened,
  source: $next,
  fn: (next) => ({ path: next, params: {}, query: {}, method: 'push' }) as const,
  // target: router.push,
  target: openRoute,
});
sample({
  clock: resetPasswordMutation.finished.success,
  source: $next,
  fn: (next) => ({ path: next, params: {}, query: {}, method: 'push' }) as const,
  target: openRoute,
});
// Logout
sample({
  clock: logout,
  target: loginRoute.open,
});
sample({
  clock: logout,
  source: registrationForm.$form,
  target: createEffect((form: any) => form.reset()),
});
// Forgot(reset) password
const ResetPassDto = z.object({
  newPassword: z
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
  email: z.string(),
  resetCode: z.string(),
});
const ForgotPassDto = z.object({
  email: z.string().email('Введите корректную почту'),
});
export const resetPassForm = createForm({
  defaultValues: {
    newPassword: '',
    email: new URLSearchParams(window.location.search).get('email') || '',
    resetCode: new URLSearchParams(window.location.search).get('token') || '',
  },
  validators: {
    onSubmit: ResetPassDto,
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
export const forgotPassForm = createForm({
  defaultValues: {
    email: '',
  },
  validators: {
    onSubmit: ForgotPassDto,
    onBlur: ForgotPassDto,
  },
});
submit(resetPasswordMutation, {
  on: resetPassForm,
});
submit(forgotPasswordMutation, {
  on: forgotPassForm,
});
{
  querySync({
    source: { step: $step },
    route: routes.resetpass,
    cleanup: {
      irrelevant: true,
      empty: true,
    },
    controls,
  });
  sample({
    clock: $step,
    filter: (step) => step !== null && ['reset'].includes(step),
    target: $loginStep,
  });

  // Protection incorrect URL
  sample({
    clock: routes.resetpass.opened,
    source: {
      values: resetPassForm.$values,
      step: $step,
    },
    filter: ({ values, step }) => {
      if (step !== 'reset') return false;
      const email = values?.email || '';
      return values?.resetCode === '' || !email.includes('@');
    },
    target: routes.signin.open,
  });

  sample({
    clock: routes.resetpass.opened,
    fn: () => null,
    target: $step,
  });
  sample({
    clock: [forgotPasswordMutation.finished.success],
    fn: () => {
      return {
        severity: 'success',
        life: 1000,
        summary: 'Ссылка для смены пароля отправлена на почту',
      } as const;
    },
    target: showToast,
  });
}
// invitation link to account
{
  const $invitationId = createStore<string>('');
  const $tokenInvitation = createStore<string>('');

  querySync({
    source: {
      invitationId: $invitationId,
      token: $tokenInvitation,
    },
    route: routes.signup,
    cleanup: {
      irrelevant: true,
      empty: true,
    },
    controls,
  });
  querySync({
    source: {
      invitationId: $invitationId,
      token: $tokenInvitation,
    },
    route: routes.signin,
    cleanup: {
      irrelevant: true,
      empty: true,
    },
    controls,
  });
  sample({
    clock: merge([routes.signup.opened.map(() => 'signup'), routes.signin.opened.map(() => 'signin')]),
    source: {
      signupForm: registrationForm.$form,
      signInForm: loginForm.$form,
      invitationId: $invitationId,
      token: $tokenInvitation,
    },
    filter: ({ signupForm, signInForm }, routeName) =>
      routeName === 'signup' ? Boolean(signupForm) : Boolean(signInForm),
    fn: ({ signupForm, signInForm, invitationId, token }, routeName) => {
      const form = (routeName === 'signup' ? signupForm : signInForm) as unknown as FormApi<Record<string, any>>;

      if (!form) return null;

      form.setFieldValue('data.invitationId', invitationId ?? '');
      form.setFieldValue('data.token', token ?? '');

      return null;
    },
  });

  sample({
    clock: [routes.signup.opened, routes.signup.updated, routes.signin.opened, routes.signin.updated],
    fn: () => '',
    target: [$invitationId, $tokenInvitation],
  });
}
