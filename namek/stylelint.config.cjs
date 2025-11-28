module.exports = {
  customSyntax: 'postcss-html',
  plugins: ['stylelint-scss'],
  extends: ['stylelint-config-recommended-vue/scss'],
  rules: {
    'selector-class-pattern': ['^([a-z][a-z0-9]*)(-[a-z0-9]+)*$', { message: 'Expected class name to be kebab-case' }],
    'selector-id-pattern': ['^[a-z][a-zA-Z0-9]+$', { message: 'Expected id name to be camelCase' }]
  }
}
