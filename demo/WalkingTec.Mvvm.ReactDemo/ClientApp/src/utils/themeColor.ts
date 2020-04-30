import client from 'webpack-theme-color-replacer/client';
import generate from '@ant-design/colors/lib/generate';
import { message } from 'antd';
import { getLocalesValue } from 'locale';

export default {
  getAntdSerials(color: string): string[] {
    const lightCount = 9;
    const divide = 10;
    // 淡化（即less的tint）
    let lightens = new Array(lightCount).fill(0);
    lightens = lightens.map((_, i) => client.varyColor.lighten(color, i / divide));
    const colorPalettes = generate(color);
    const rgb = client.varyColor.toNum3(color.replace('#', '')).join(',');
    return lightens.concat(colorPalettes).concat(rgb);
  },
  async changeColor(color?: string): Promise<void> {
    if (!color) {
      return Promise.resolve();
    }
    const options = {
      // new colors array, one-to-one corresponde with `matchColors`
      newColors: this.getAntdSerials(color),
      changeUrl(cssUrl: string = 'static/css/theme-colors.css'): string {
        console.log("changeUrl -> cssUrl", cssUrl)
        // while router is not `hash` mode, it needs absolute path
        return `/${cssUrl}`;
      },
    };
    message.loading({ content: getLocalesValue('tips.theme.loading'), key: 'changeColor' });
    const res = await client.changer.changeColor(options, Promise);
    message.destroy()
    return;
  },
};