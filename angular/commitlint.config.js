module.exports = {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'subject-empty': [2, 'never'], // Ensure the subject is not empty
    'type-empty': [2, 'never'], // Ensure the type is not empty
    'type-enum': [
      2,
      'always',
      [
        'feat', // Feature
        'fix', // Bug fix
        'docs', // Documentation changes
        'style', // Code style changes (formatting)
        'refactor', // Code refactoring
        'test', // Adding or updating tests
        'chore', // Routine tasks (e.g., config updates)
        'build', // Build process or dependencies
        'ci', // Continuous integration changes
        'perf', // Performance improvements
        'revert', // Reverting commits
      ],
    ],
  },
};
