import { useStore, useUnit } from 'effector-react';
import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import { Dropdown } from '~/shared/ui/dropdown';
import {
  $companySuggestValue,
  $companyValue,
  $industries,
  $isAnySelected,
  $roles,
  $selectedCompany,
  $selectedIndustry,
  $selectedRole,
  submitProfile,
  updateCompanyValue,
  updateIndustry,
  updateOrCreateCompany,
  updateRole,
} from '../model';
import { Link } from 'atomic-router-react';
import { routes } from '~/shared/routing';

export const SignUpInfo = () => {
  const [industries, selectedIndustry, updateIndustryCall] = useUnit([$industries, $selectedIndustry, updateIndustry]);
  const [roles, selectedRole, updateRoleCall] = useUnit([$roles, $selectedRole, updateRole]);
  const submit = useUnit(submitProfile);
  const isAnySelected = useUnit($isAnySelected);

  return (
    <div className="flex flex-column align-items-center mt-8">
      <h1 className="mt-7 mb-4 max-w-29rem font-medium text-2xl">О себе</h1>
      <span className="text-center mb-3 text-sm line-height-3">
        Расскажите нам немного о себе.
        <br /> Эти данные можно заполнить позже в профиле
      </span>
      <article className="flex flex-column w-full sm:w-21rem justify-content-center gap-2">
        <div className="text-color-secondary flex flex-column gap-2">
          <section className="flex flex-column gap-1">
            <label htmlFor="company">Компания</label>
            <SelectCompany />
          </section>
          <section className="flex flex-column gap-1">
            <label htmlFor="role">Роль</label>
            <Dropdown
              id="role"
              placeholder="Выберите"
              value={selectedRole}
              onChange={(e) => {
                updateRoleCall(e.value);
              }}
              options={roles}
              optionLabel="name"
              optionValue="id"
              optionGroupLabel="label"
              optionGroupChildren="items"
            />
          </section>
          <section className="flex flex-column gap-1">
            <label htmlFor="industry">Индустрия</label>
            <Dropdown
              id="industry"
              placeholder="Выберите"
              value={selectedIndustry}
              onChange={(e) => {
                updateIndustryCall(e.value);
              }}
              options={industries}
              optionLabel="name"
              optionValue="id"
            />
          </section>
        </div>
      </article>
      <Button onClick={submit} label="Сохранить" disabled={!isAnySelected} className="sm:w-21rem w-full mt-4" />
      <Button
        onClick={submit}
        label="Пропустить"
        className="sm:w-21rem w-full mt-3 mb-4"
        size="small"
        severity="secondary"
      />
      <span>
        Уже есть профиль?{' '}
        <Link to={routes.signin} className="text-primary">
          Войти
        </Link>
      </span>
    </div>
  );
};

function SelectCompany() {
  const model = useUnit({ value: $companyValue, update: updateCompanyValue });
  const suggest = useUnit($companySuggestValue);
  const updateCompanyCall = useUnit(updateOrCreateCompany);

  return (
    <div className="relative" style={{ height: '32px' }}>
      <InputText className="absolute text-400 pointer-events-none w-full" value={suggest} />
      <InputText
        className="absolute w-full"
        placeholder="Введите"
        style={{ backgroundColor: 'transparent' }}
        onChange={(e) => model.update({ companyName: e.target.value })}
        onKeyDown={(k) => {
          if (k.code === 'ArrowRight' || k.code === 'Enter') {
            updateCompanyCall();
          }
        }}
        value={model.value ?? ''}
      />
    </div>
  );
}
