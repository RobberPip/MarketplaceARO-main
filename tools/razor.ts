import { createLogger, PluginOption } from 'vite';
import path from 'node:path';
import fs from 'node:fs';

export const prepareRasorIndex: PluginOption = {
  name: 'razor-index',
  transformIndexHtml(html, { path }) {
    if (path.includes('razor.html')) {
      return html
        .replace(/\s\s<script async type="module" crossorigin src="\/app\/assets.*"><\/script>\n/g, '')
        .replace(/__styles__/g, `<link rel="stylesheet" href="~/css/site.css" />`);
    }

    return html;
  },
};

export const updateRazorAssets = (): PluginOption => {
  const logger = createLogger();

  return {
    name: 'razor-assets',
    apply: 'build',
    async closeBundle() {
      const razorAssets = path.resolve(__dirname, '../backend/App/wwwroot/app');
      const dist = path.resolve(__dirname, '../frontend/dist');
      const razorLayout = path.resolve(__dirname, '../backend/Market/Pages/Shared/_Layout.cshtml');
      const legacyLinks = path.resolve(__dirname, '../backend/RegistryApp/Views/Shared/_Links.cshtml');
      const index = path.resolve(__dirname, '../frontend/dist/razor.html');
      const legacyIndex = path.resolve(__dirname, '../frontend/dist/profile-widget.html');

      try {
        // Legacy _Layout.cshtml

        const sourceHtml = await fs.promises.readFile(legacyIndex, { encoding: 'utf-8' });
        const scriptRegex =
          /<script type="module" crossorigin src="(\/app\/assets\/profile-widget-[^\"]+\.js)"><\/script>/;
        const cssRegex = /<link rel="stylesheet" crossorigin href="(\/app\/assets\/index-[^\"]+\.css)">/;

        const scriptMatch = sourceHtml.match(scriptRegex);

        const cssMatch = sourceHtml.match(cssRegex);

        const scriptSrc = scriptMatch ? scriptMatch[1] : null;
        const cssSrc = cssMatch ? cssMatch[1] : null;

        const links = `    <link rel="stylesheet" crossorigin href="~${cssSrc}" />
    <script type="module" defer src="~${scriptSrc}"></script>`;

        await fs.promises.writeFile(legacyLinks, links, { encoding: 'utf-8' });

        // _Layout.cshtml
        // TODO: refactor like legacy: <partial name="_Links" optional="true"/>
        if (fs.existsSync(razorLayout)) {
          await fs.promises.rm(razorLayout);
        }
        await fs.promises.rename(index, razorLayout);

        // dist -> wwwroot/app
        if (fs.existsSync(razorAssets)) {
          await fs.promises.rm(razorAssets, { recursive: true, force: true });
        }
        await fs.promises.rename(dist, razorAssets);

        logger.info('Razor assets and layout successfully updated.');
      } catch (error) {
        logger.error('Error updating Razor assets', { error });
      }
    },
  };
};
