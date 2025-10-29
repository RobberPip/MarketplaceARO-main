import { useUnit } from 'effector-react';
import { Divider } from 'primereact/divider';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { Message } from 'primereact/message';

import { useState } from 'react';
import { $activeAccount, $suggestions, fetchSuggestionsRaw, productForm } from '../model';
import { AutoComplete } from 'primereact/autocomplete';
import { components } from '~/shared/api/schema';
import { ImageFileUpload } from '~/widgets/image-upload/ImageUpload';
import { UrlInput } from '~/shared/ui/components/url-input';
import { ProductIn } from '../validation';
import { z } from 'zod';

export const GeneralInformation = () => {
  const { extendedForm: form } = useUnit(productForm);
  const [showChips, setShowChips] = useState(false);
  const activeAccount:
    | (components['schemas']['GetAccountSummaryOut'] & { brand?: { publicName?: string | null } })
    | null = useUnit($activeAccount);
  const developerName = activeAccount?.brand?.publicName ?? activeAccount?.shortName ?? '';
  const rightholderName = activeAccount?.shortName;
  const [suggestions, fetch] = useUnit([$suggestions, fetchSuggestionsRaw]);
  return (
    <div className="flex flex-column gap-5">
      <div className="flex flex-column">
        <h3 className="m-0 text-lg font-medium">Общая информация</h3>
        <Divider className="mt-2 mb-3" />
        {/* <section className="flex flex-column gap-1">
          <label htmlFor="lifecycleStage" className="text-color-secondary text-sm">
            Тип продукта*
          </label>
          <Dropdown
            // value={lifecycleStageField.state.value}
            // options={lifecycleStages}
            placeholder="Выберите из списка"
            // onChange={(e) => lifecycleStageField.handleChange(e.value)}
            className="w-28rem"
          />
          <p className="text-color-secondary text-xs mt-1 mb-3 ">
            Поле определяет, какой способ использования и размещения ПО вы предлагаете.
            <br />
            <span className="font-semibold">Standalone</span> — автономное ПО, которое работает независимо на устройстве
            пользователя без необходимости постоянного подключения к сети.
            <br />
            <span className="font-semibold">On-Premise</span> — ПО устанавливается и работает на серверах клиента,
            обеспечивая полный контроль над данными и инфраструктурой.
            <br />
            <span className="font-semibold">SaaS</span> — ПО предоставляется как облачный сервис, размещённый и
            полностью управляемый на серверах вендора, доступный через интернет без установки
          </p>
        </section> */}
        <section className="flex flex-column gap-1 mb-3">
          <label className="text-color-secondary text-sm">Разработчик</label>
          <InputText value={developerName ?? ''} placeholder="Введите" className="w-28rem" disabled />
        </section>
        <section className="flex flex-column gap-1 mb-3">
          <form.Field
            name="rightholders"
            children={(field) => {
              const selectedElements = (field.state.value || []) as components['schemas']['GetAccountSummaryOut'][];
              const displayRightholders = selectedElements.filter((el) => el.id && el.shortName !== rightholderName);
              const showAddButton = !showChips && displayRightholders.length === 0;
              const selectedIds = new Set(selectedElements.map((el) => el.id).filter((id): id is string => !!id));
              const filteredSuggestions = suggestions.filter((s) => s.id && !selectedIds.has(s.id));
              return (
                <>
                  <label className="text-color-secondary text-sm">
                    Правообладатель
                    <InputText placeholder="Введите" disabled className="mt-1 w-28rem" value={rightholderName ?? ''} />
                  </label>

                  {showAddButton ? (
                    <span className="text-blue-600 text-sm cursor-pointer" onClick={() => setShowChips(true)}>
                      + Добавить правообладателя
                    </span>
                  ) : (
                    <>
                      <label className="text-color-secondary text-sm mt-2">Другие правообладатели</label>
                      <AutoComplete
                        value={displayRightholders ?? []}
                        suggestions={filteredSuggestions}
                        completeMethod={(e) => fetch(e.query)}
                        multiple
                        key="id"
                        field="shortName"
                        onChange={(e) => {
                          const newItems = (e.value || []).filter(
                            (item: { id: string }): item is { id: string } =>
                              typeof item.id === 'string' && item.id !== '',
                          );
                          field.handleChange(newItems);
                        }}
                        className="w-28rem"
                        placeholder="Введите"
                      />
                    </>
                  )}
                </>
              );
            }}
          />
        </section>
        <section className="flex flex-column gap-1 mb-2">
          <form.Field
            name="description"
            children={(field) => (
              <>
                <label htmlFor={field.name} className="text-color-secondary text-sm">
                  Описание
                </label>
                <InputTextarea
                  id={field.name}
                  name={field.name}
                  value={field.state.value ?? ''}
                  rows={3}
                  placeholder="Введите краткое описание продукта"
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                  className="w-full"
                />
              </>
            )}
          />
        </section>
        <section className="flex flex-column gap-1 w-28rem">
          <form.Field
            name="website"
            validators={{
              onChange: ({ value }) => {
                if (!value || value.trim() === '') {
                  return undefined;
                }
                try {
                  ProductIn.shape.website.parse(value);
                  return undefined;
                } catch (error) {
                  if (error instanceof z.ZodError) {
                    return error.errors[0].message;
                  }
                  return 'Ошибка валидации';
                }
              },
            }}
            children={(field) => (
              <>
                <label htmlFor={field.name} className="text-color-secondary text-sm">
                  Сайт
                </label>
                <UrlInput field={field} placeholder="https://" />
              </>
            )}
          />
        </section>
      </div>
      <div className="flex flex-column">
        <h3 className="m-0 text-lg font-medium">Фото и видео</h3>
        <Divider className="mt-2 mb-3" />
        <div className="flex flex-column gap-3">
          <section className="gap-2 text-xs">
            <span className="text-color-secondary text-xs">
              Нажмите на кнопку или перетащите в эту область до 10 изображений в формате JPEG или PNG. Размер каждого
              файла не должен превышать 2,5 мб и иметь разрешение
              <br />
              не больше 1000 px по длинной стороне
            </span>
            <form.Field
              name="images"
              children={(field) => (
                <ImageFileUpload
                  className="mt-4"
                  form={form}
                  fieldName="images"
                  initialFileUrls={field.state.value ?? []}
                />
              )}
            />
          </section>
          <section className="flex flex-column mb-5">
            <form.Field
              name="productData.VideoUrl"
              children={(field) => (
                <>
                  <label className="text-color-secondary text-sm mt-2">Источник видео</label>
                  <InputText
                    placeholder="Введите"
                    id={field.name}
                    name={field.name}
                    value={field.state.value ?? ''}
                    onBlur={field.handleBlur}
                    onChange={(e) => field.handleChange(e.target.value)}
                    invalid={field.state.meta.errors.length > 0}
                    className="mt-1 w-28rem"
                  />

                  {field.state.meta.errors.length > 0 && (
                    <Message className="w-full mt-1" severity="error" text={field.state.meta.errors.join('\n')} />
                  )}
                </>
              )}
            />
            <span className="text-color-secondary text-xs mt-2">
              Добавьте ссылку на видео с онлайн-сервиса Rutube или VK Video, или введите его ID. Видео должно быть с
              публичным доступом
            </span>
          </section>
        </div>
      </div>
      {/* <div className="flex flex-column">
        <h2 className="text-xl font-medium">Характеристики ПО</h2>
        <Divider className="mt-3 mb-4" />
        <div className="flex flex-column gap-3">
          <section className="flex align-items-center gap-2 text-xs">
            <div className="text-color-secondary w-10rem">Производитель</div>
            <div>CSoft Development</div>
          </section>
          <section className="flex align-items-center gap-2 text-xs">
            <div className="text-color-secondary w-10rem">Применение</div>
            {applicationTypeOptions.map((option) => (
              <div key={option.value} className="flex align-items-center">
                <RadioButton
                  inputId={option.value}
                  name="applicationType"
                  value={option.value}
                  checked={applicationTypeField.state.value === option.value}
                  onChange={(e) => applicationTypeField.handleChange(e.value)}
                />
                <label htmlFor={option.value} className="ml-2">
                  {option.label}
                </label>
              </div>
            ))}
          </section>

          {applicationTypeField.state.value === 'Plugin' && (
            <section className="flex flex-column gap-1">
              <label htmlFor="mainProduct" className="text-color-secondary text-xs">
                Основной продукт
              </label>
              <MultiSelect
                value={mainProductField.state.value}
                options={projectMarks}
                placeholder="Выберите основной продукт"
                display="chip"
                onChange={(e) => mainProductField.handleChange(e.value)}
                className="w-28rem"
              />
            </section>
          )}

          <section className="flex justify-content-between w-full align-items-center">
            <div className="flex flex-column">
              <h3 className="m-0 font-semibold text-base">Иконка</h3>
              <div className="text-color-secondary text-xs mt-2">Описание, характеристики изображения</div>
            </div>
            {iconField.state.value ? (
              <Avatar
                className="p-overlay-badge"
                image={
                  typeof iconField.state.value === 'string'
                    ? iconField.state.value
                    : URL.createObjectURL(iconField.state.value)
                }
                size="xlarge"
                shape="circle"
              >
                <Badge
                  value="X"
                  severity="contrast"
                  className="cursor-pointer"
                  onClick={() => iconField.handleChange('')}
                />
              </Avatar>
            ) : (
              <FileUpload
                mode="basic"
                accept="image/*"
                maxFileSize={1000000}
                customUpload
                chooseLabel="Добавить иконку"
                onSelect={(e) => iconField.handleChange(e.files[0])}
              />
            )}
          </section>
        </div>
      </div> */}

      {/* <div>
        <h2 className="text-xl font-medium">Классификация</h2>
        <Divider className="mt-3 mb-4" />

        <div className="flex flex-column gap-3">
          <section className="flex flex-column gap-1">
            <label htmlFor="lifecycleStage" className="text-color-secondary text-xs">
              Стадия жизненного цикла
            </label>
            <MultiSelect
              value={lifecycleStageField.state.value}
              options={lifecycleStages}
              placeholder="Выберите"
              onChange={(e) => lifecycleStageField.handleChange(e.value)}
              className="w-28rem"
            />
          </section>

          <section className="flex flex-column gap-1">
            <label htmlFor="businessProcess" className="text-color-secondary text-xs">
              Бизнес-процесс
            </label>
            <InputText
              value={businessProcessField.state.value}
              placeholder="Введите бизнес процесс"
              onChange={(e) => businessProcessField.handleChange(e.target.value)}
              className="w-28rem"
            />
          </section>

          <section className="flex flex-column gap-1">
            <label htmlFor="class" className="text-color-secondary text-xs">
              Класс
            </label>
            <Dropdown
              value={classField.state.value}
              options={classes}
              placeholder="Выберите класс"
              onChange={(e) => classField.handleChange(e.value)}
              className="w-28rem"
            />
          </section>

          <section className="flex flex-column gap-1">
            <label htmlFor="impactObject" className="text-color-secondary text-xs">
              Объект воздействия
            </label>
            <MultiSelect
              value={impactObjectsField.state.value}
              options={impactObjects}
              placeholder="Выберите объект воздействия"
              display="chip"
              onChange={(e) => impactObjectsField.handleChange(e.value)}
              className="w-28rem"
            />
          </section>

          <section className="flex flex-column gap-1">
            <label htmlFor="projectMark" className="text-color-secondary text-xs">
              Марка проекта
            </label>
            <MultiSelect
              value={projectMarksField.state.value}
              options={projectMarks}
              placeholder="Выберите марку проекта"
              display="chip"
              onChange={(e) => projectMarksField.handleChange(e.value)}
              className="w-28rem"
            />
          </section>
        </div>
      </div>

      <div>
        <h2 className="text-xl font-medium">Совместимость</h2>
        <Divider className="mt-3 mb-4" />

        <div className="flex flex-column gap-3">
          <section className="flex items-center">
            <Checkbox
              inputId="worksWithCIM"
              checked={worksWithCIMField.state.value}
              onChange={(e) => worksWithCIMField.handleChange(Boolean(e.checked))}
            />
            <label htmlFor="worksWithCIM" className="text-color-secondary text-xs ml-2">
              Работает с ЦИМ
            </label>
          </section>
        </div>
      </div>

      <div>
        <h2 className="text-xl font-medium">Публичные списки</h2>
        <Divider className="mt-3 mb-4" />

        <div className="flex flex-column gap-3">
          <section className="flex items-center">
            <Checkbox
              inputId="inNOTIM"
              checked={inNOTIMField.state.value}
              disabled
              onChange={(e) => inNOTIMField.handleChange(Boolean(e.checked))}
            />
            <label htmlFor="inNOTIM" className="text-color-secondary text-xs ml-2">
              В реестре НОТИМ
            </label>
          </section>

          <section className="flex items-center">
            <Checkbox
              inputId="inRegistry"
              checked={inRegistryField.state.value}
              onChange={(e) => inRegistryField.handleChange(Boolean(e.checked))}
            />
            <label htmlFor="inNOTIM" className="text-color-secondary text-xs ml-2">
              В реестре отечественного ПО
            </label>
          </section>

          {inRegistryField.state.value && (
            <>
              <section className="flex flex-column gap-1">
                <label htmlFor="registryCode" className="text-color-secondary text-xs">
                  Код записи в реестре отечественного ПО
                </label>
                <InputText
                  value={registryCodeField.state.value}
                  placeholder="Введите код"
                  onChange={(e) => registryCodeField.handleChange(e.target.value)}
                  className="w-28rem"
                />
              </section>
              <section className="flex flex-column gap-1">
                <label htmlFor="registryName" className="text-color-secondary text-xs">
                  Имя в реестре отечественного ПО
                </label>
                <InputText
                  value={registryNameField.state.value}
                  placeholder="Введите имя"
                  onChange={(e) => registryNameField.handleChange(e.target.value)}
                  className="w-28rem"
                />
              </section>
              <section className="flex flex-column gap-1">
                <label htmlFor="registryLink" className="text-color-secondary text-xs">
                  Ссылка на запись в реестре отечественного ПО
                </label>
                <InputText
                  value={registryLinkField.state.value}
                  placeholder="https://"
                  onChange={(e) => registryLinkField.handleChange(e.target.value)}
                  className="w-28rem"
                />
              </section>
            </>
          )}
        </div>
      </div>

      <div className="flex justify-content-around align-items-center gap-8 px-5 mt-8">
        <Button label="Отменить" onClick={prev} size="large" outlined className="w-full" />
        <Button
          label="Далее"
          onClick={(e) => {
            e.preventDefault();
            next();
          }}
          size="large"
          className="w-full"
        />
      </div> */}
    </div>
  );
};
