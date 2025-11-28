/** @type {import('tailwindcss').Config} */
import { primitive } from './primitive'

const semanticColorsFromPrimitive = (names) =>
  Object.fromEntries(
    names.map((name) => [
      name,
      {
        DEFAULT: primitive.colors[name][500],
        ...Object.fromEntries(Object.keys(primitive.colors[name]).map((key) => [key, primitive.colors[name][key]]))
      }
    ])
  )

export default {
  prefix: 'tw-',
  content: ['./index.html', './node_modules/flowbite/**/*.js', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    fontWeight: {
      regular: '400',
      bold: '700'
    },
    fontFamily: {
      DEFAULT: ['Sarabun', 'sans-serif']
    },
    fontSize: {
      textLink: ['16px', '26px'],
      'title-small-reg': ['22px', { lineHeight: '36px', fontWeight: '400' }],
      'title-small-bold': ['22px', { lineHeight: '36px', fontWeight: '600' }],
      'title-medium-reg': ['28px', { lineHeight: '46px', fontWeight: '400' }],
      'title-medium-bold': ['28px', { lineHeight: '46px', fontWeight: '700' }],
      'body-large-reg': ['18px', { lineHeight: '30px', fontWeight: '400' }],
      'body-large-bold': ['18px', { lineHeight: '30px', fontWeight: '600' }],
      'body-medium-reg': ['16px', { lineHeight: '26px', fontWeight: '400' }],
      'body-medium-reg-narrow': ['16px', { lineHeight: '18px', fontWeight: '400' }],
      'body-medium-bold': ['16px', { lineHeight: '26px', fontWeight: '700' }],
      'body-medium-bold-narrow': ['16px', { lineHeight: '18px', fontWeight: '700' }],
      'body-small-reg-narrow': ['12px', { lineHeight: '16px', fontWeight: '400' }],
      'body-small-reg': ['12px', { lineHeight: '19px', fontWeight: '400' }],
      'body-small-bold-narrow': ['12px', { lineHeight: '16px', fontWeight: '700' }],
      'body-small-bold': ['12px', { lineHeight: '19px', fontWeight: '700' }],
      'body-xsmall-reg': ['10px', { lineHeight: '16px', fontWeight: '400' }],
      'body-xsmall-bold': ['10px', { lineHeight: '16px', fontWeight: '600' }]
    },
    tableLayout: ['responsive', 'hover', 'focus'],
    colors: {
      white: primitive.colors.white,
      black: primitive.colors.black,
      ...semanticColorsFromPrimitive(['gray', 'navy', 'red', 'orange', 'yellow', 'purple', 'blue', 'green']),
      background: {
        default: primitive.colors.white,
        subtle: primitive.colors.gray[25],
        hover: primitive.colors.gray[100],
        disabled: primitive.colors.gray[50],
        brand: {
          default: primitive.colors.navy[500],
          'default-hover': primitive.colors.navy[700],
          subtle: primitive.colors.navy[50],
          'subtle-hover': primitive.colors.navy[100]
        },
        informative: {
          default: primitive.colors.blue[500],
          'default-hover': primitive.colors.blue[700],
          subtle: primitive.colors.blue[50],
          'subtle-hover': primitive.colors.blue[100]
        },
        positive: {
          default: primitive.colors.green[500],
          'default-hover': primitive.colors.green[700],
          subtle: primitive.colors.green[50],
          'subtle-hover': primitive.colors.green[200]
        },
        warning: {
          default: primitive.colors.yellow[500],
          'default-hover': primitive.colors.yellow[600],
          subtle: primitive.colors.yellow[50],
          'subtle-hover': primitive.colors.yellow[200]
        },
        negative: {
          default: primitive.colors.red[500],
          'default-hover': primitive.colors.red[700],
          subtle: primitive.colors.red[50],
          'subtle-hover': primitive.colors.red[200]
        },
        ...semanticColorsFromPrimitive(['gray', 'navy', 'red', 'orange', 'yellow', 'purple', 'blue', 'green'])
      },
      gray: primitive.colors.gray,
      content: {
        default: primitive.colors.gray[800],
        subtle: primitive.colors.gray[500],
        disabled: primitive.colors.gray[400],
        inverted: primitive.colors.white,
        brand: {
          default: primitive.colors.navy[500],
          strong: primitive.colors.navy[700]
        },
        informative: {
          default: primitive.colors.blue[500],
          strong: primitive.colors.blue[700]
        },
        positive: {
          default: primitive.colors.green[500],
          strong: primitive.colors.green[700]
        },
        negative: {
          default: primitive.colors.red[500],
          strong: primitive.colors.red[700]
        },
        warning: {
          default: primitive.colors.yellow[500],
          strong: primitive.colors.yellow[800]
        }
      }
    },
    boxShadow: {
      'bottom-100': primitive.shadow['bottom-100'],
      'bottom-200': primitive.shadow['bottom-200'],
      'bottom-300': primitive.shadow['bottom-300'],
      'bottom-400': primitive.shadow['bottom-400']
    },
    extend: {
      animation: {
        'spin-fast': 'spin .6s linear infinite'
      },
      padding: {
        null: primitive.spacing.null,
        '3xsmall': primitive.spacing['3xsmall'],
        '2xsmall': primitive.spacing['2xsmall'],
        xsmall: primitive.spacing.xsmall,
        small: primitive.spacing.small,
        medium: primitive.spacing.medium,
        large: primitive.spacing.large,
        xlarge: primitive.spacing.xlarge,
        '2xlarge': primitive.spacing['2xlarge']
      },
      borderRadius: {
        '3xsmall': primitive['corner-radius']['3xsmall'],
        '2xsmall': primitive['corner-radius']['2xsmall'],
        xsmall: primitive['corner-radius'].xsmall,
        small: primitive['corner-radius'].small,
        medium: primitive['corner-radius'].medium,
        large: primitive['corner-radius'].large,
        full: primitive['corner-radius'].full
      },
      borderColor: {
        default: primitive.colors.gray[200],
        'default-hover': primitive.colors.gray[500],
        'default-active': primitive.colors.gray[600],
        inverted: primitive.colors.white,
        focus: primitive.colors.blue[100],
        brand: {
          default: primitive.colors.navy[500],
          subtle: primitive.colors.navy[300]
        },
        informative: {
          default: primitive.colors.blue[500],
          subtle: primitive.colors.blue[300]
        },
        positive: {
          default: primitive.colors.green[500],
          subtle: primitive.colors.green[300]
        },
        warning: {
          default: primitive.colors.yellow[500],
          subtle: primitive.colors.yellow[300]
        },
        negative: {
          default: primitive.colors.red[500],
          subtle: primitive.colors.red[300]
        }
      },
      gap: {
        null: primitive.spacing.null,
        '3xsmall': primitive.spacing['3xsmall'],
        '2xsmall': primitive.spacing['2xsmall'],
        xsmall: primitive.spacing.xsmall,
        small: primitive.spacing.small,
        medium: primitive.spacing.medium,
        large: primitive.spacing.large,
        xlarge: primitive.spacing.xlarge,
        '2xlarge': primitive.spacing['2xlarge']
      },
      gridTemplateColumns: {
        layout: 'repeat(4, minmax(50px, 1fr))',
        custom: 'repeat(4, minmax(100px, 1fr))',
        cols1: 'repeat(1, minmax(100px, 2fr))',
        cols2: 'repeat(2, minmax(100px, 2fr))',
        cols3: 'repeat(3, minmax(100px, 2fr))',
        cols4: 'repeat(4, minmax(100px, 2fr))',
        cols5: 'repeat(5, minmax(100px, 2fr))',
        cols6: 'repeat(6, minmax(100px, 2fr))',
        cols7: 'repeat(7, minmax(100px, 2fr))',
        cols8: 'repeat(8, minmax(100px, 2fr))',
        cols9: 'repeat(9, minmax(100px, 2fr))',
        cols10: 'repeat(10, minmax(100px, 2fr))',
        cols11: 'repeat(11, minmax(100px, 2fr))',
        cols12: 'repeat(12, minmax(100px, 2fr))'
      },
      gridTemplateRows: {
        layout: 'repeat(4, minmax(50px, 1fr))',
        custom: 'repeat(1, minmax(100px, 3fr))',
        rows1: 'repeat(1, minmax(100px, 2fr))',
        rows2: 'repeat(2, minmax(100px, 2fr))',
        rows3: 'repeat(3, minmax(100px, 2fr))',
        rows4: 'repeat(4, minmax(100px, 2fr))',
        rows5: 'repeat(5, minmax(100px, 2fr))',
        rows6: 'repeat(6, minmax(100px, 2fr))',
        rows7: 'repeat(7, minmax(100px, 2fr))',
        rows8: 'repeat(8, minmax(100px, 2fr))',
        rows9: 'repeat(9, minmax(100px, 2fr))',
        rows10: 'repeat(10, minmax(100px, 2fr))',
        rows11: 'repeat(11, minmax(100px, 2fr))',
        rows12: 'repeat(12, minmax(100px, 2fr))'
      }
    }
  },
  plugins: []
}

