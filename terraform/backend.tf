terraform {
  backend "s3" {
    bucket       = "terraform-tfstate-ker4ic"
    key          = "terraform.tfstate"
    region       = "eu-central-1"
    use_lockfile = true
    encrypt      = true
  }
}
