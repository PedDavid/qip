# Contribution Guide

## Concepts

### [Sematic Versioning 2.0](http://semver.org/spec/v2.0.0.html)

>Given a version number MAJOR.MINOR.PATCH, increment the:
>
 >1. MAJOR version when you make incompatible API changes,
 >2. MINOR version when you add functionality in a backwards-compatible manner,
 >3. PATCH version when you make backwards-compatible bug fixes
>
>Specification
>
 >4. Major version zero (0.y.z) is for initial development. Anything may change at any time. The public API should not be considered stable.

### [Git Flow][git flow] branching model.

Two main branches with an infinite lifetime:

- `master` (reserved for deployment)
- `develop`

Both branches are [Protected](https://help.github.com/articles/about-protected-branches/)

`master` requires a [Pull Request Review](https://help.github.com/articles/about-pull-request-reviews/)

`develop` is submitted to **Continuous Integration**. Making it somewhat stable to be considered as a *nightly build*.

Since **status checks** require another branch (you can't commit directly to develop), project development is done through **support branches**. 

**Support Branches naming conventions:**

- format `tag/description`
- tags `feature`, `hot-fix` and `release` are reserved by [Git Flow][git flow]
- description `addt-<technology-name>` when adding a new technology for easier pin-pointing

## In Practice

### Features

1. Create a new **feature-branch**, branching from `develop`

   `git checkout -b feature/x develop`

2. Work on feature. **Commit often!** 

    Follow [The Seven rules of a grat Git commit message](https://chris.beams.io/posts/git-commit/#seven-rules)

    [Strive for atomic commits](https://www.freshconsulting.com/atomic-commits/).
    Smaller commits have more precise diffs and a clear sequence, thus are easier to understand. 

    `git commit -m "Descriptive change message"`

3. "Merge" 

   1. Create a **Pull Request**

    **or**

   1. Push features to remote

        `git push -u origin feature/x`

   2. Change back to develop

        `git checkout develop`

   3. Merge changes

        `git merge --no-ff feature/x`

        Optionally delete local branch

        `git branch -d feature/x`

   4. Push merge to remote 

        `git push origin develop`

        This could give an error if mandatory **status checks** are still pending or failed

### Releases

Follow a similar pattern to features with small changes

1. naming convention is `release/version`

2. Bumps version across configuration files

3. Should only do last touch commits (e.g., bugfixes, deployment configurations...)

4. Also merges with `master` branch, tagging it

## Code Conventions

Linters and static analysis tools will be incorporated into the **Continuous Integration** pipeline to enforce code conventions

### TODOs

`TODO(user): do something`

Where user should be an id of the author (e.g., github username)

You can be more suggestive about the context using the following words:

- `FIXME`
- `HACK`
- `BUG`


[git flow]: http://nvie.com/posts/a-successful-git-branching-model/