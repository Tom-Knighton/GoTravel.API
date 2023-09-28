pipeline {
  agent any

  stages {
    stage("Clean") {
        steps {
            script {
                withDotNet(sdk: '.NET 7') {
                    echo "Cleaning project..."
                    dotnetClean sdk: '.NET 7'
                }
            }
        }
    }

    stage("Add source") {
        steps {
            script {
                withDotNet(sdk: '.NET 7') {
                    withCredentials([string(credentialsId: 'addSourceCmd', variable: 'addSourceCmd')]) {
                        try {
                            echo "Adding private sources"
                            sh '$addSourceCmd'
                        } catch (err) {
                            echo "Source already exists"
                        }
                    }
                }
            }
        }
    }

    stage("Restore Project") {
        steps {
            script {
                withDotNet(sdk: '.NET 7') {
                    echo "Restoring Project"
                    dotnetRestore sdk: '.NET 7'
                }
            }
        }
    }

    stage("Unit Tests") {
        steps {
            script {
                withDotNet(sdk: '.NET 7') {
                    echo "Running Unit Tests"
                    dotnetTest sdk: '.NET 7'
                }
            }
        }
    }

    stage ("Build") {
        steps {
            script {
                withDotNet(sdk: '.NET 7') {
                    echo "Building..."
                    dotnetBuild configuration: 'Release', noRestore: true, sdk: '.NET 7'
                }
            }
        }
    }

    stage ("Docker") {
        steps {
            script {
                echo "Building docker image"
                if (env.BRANCH_NAME == "main") {
                    sh "docker build -t golondonapi:latest -f Dockerfile.prod . --progress=plain"
                    sh "docker save -o golondonapi.tar golondonapi:latest"
                }

                if (env.BRANCH_NAME == "develop") {
                    sh "docker build -t golondonapidev:latest -f Dockerfile.dev . --progress=plain"
                    sh "docker save -o golondonapi_dev.tar golondonapidev:latest"
                }
            }
        }
    }

    stage("Publish") {
        steps {
            script {
                if (env.BRANCH_NAME == 'main') {
                    sshPublisher(
                        publishers: [
                            sshPublisherDesc(
                                configName: 'VPS',
                                verbose: true,
                                transfers: [
                                    sshTransfer(
                                        sourceFiles: "golondonapi.tar",
                                        remoteDirectory: 'GoLondon.API',
                                        execTimeout: 600000,
                                        execCommand: './_scripts/glapi.sh'
                                    )
                                ]
                            )
                        ]
                    )
                }

                if (env.BRANCH_NAME == 'develop') {
                    sshPublisher(
                        publishers: [
                            sshPublisherDesc(
                                configName: 'VPS',
                                verbose: true,
                                transfers: [
                                    sshTransfer(
                                        sourceFiles: "golondonapi_dev.tar",
                                        remoteDirectory: 'GoLondon.API.Dev',
                                        execTimeout: 600000,
                                        execCommand: './_scripts/glapi_dev.sh'
                                    )
                                ]
                            )
                        ]
                    )
                }
            }
    }
  }
}