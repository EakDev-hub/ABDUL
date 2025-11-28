const { defineConfig } = require('eslint-define-config')

module.exports = defineConfig({
  root: true,
  env: {
    browser: true,
    node: true
  },
  extends: [
    'eslint:recommended',
    'plugin:vue/vue3-recommended',
    '@vue/typescript/recommended',
    'plugin:prettier/recommended'
  ],
  plugins: ['prettier'],
  ignorePatterns: ['src/assets/**/*'],
  rules: {
    'vue/multi-word-component-names': 'off',
    'no-trailing-spaces': 'error',
    'no-console': process.env.NODE_ENV !== 'production' ? 'off' : 'warn',
    'no-debugger': process.env.NODE_ENV !== 'production' ? 'off' : 'error',
    quotes: ['error', 'single', { 'avoidEscape': true, 'allowTemplateLiterals': true }],
    semi: ['error', 'never'],
    'vue/no-unused-vars': 'warn',
    'no-unused-vars': 'off',
    '@typescript-eslint/no-unused-vars': 'off',
    '@typescript-eslint/no-explicit-any': 'off',
    '@typescript-eslint/no-this-alias': 'off',
    'vue/max-attributes-per-line': 'off',
    'vue/singleline-html-element-content-newline': 'off',
    'vue/html-self-closing': 'off',
    'vue/html-closing-bracket-newline': 'off',
    'vue/no-v-html': 'off',
    'vue/valid-v-slot': 'off',
    'vue/first-attribute-linebreak': 'off',
    'vue/attribute-hyphenation': 'off',
    'vue/comment-directive': 'off',
    'prettier/prettier': 'off'
  }
})
